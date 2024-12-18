using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Putov_backend.Models;
using Putov_backend.Dtos;
using Putov_backend.Data;

[Route("api/[controller]")]
[ApiController]
public class ParticipantController : ControllerBase
{
    private readonly AppDbContext _context;

    public ParticipantController(AppDbContext context)
    {
        _context = context;
    }

    // Получить всех участников
    [HttpGet]
    public ActionResult<IEnumerable<Participant>> GetParticipants()
    {
        return Ok(_context.Participants.Include(p => p.Trainings).ToList());
    }

    // Получить участника по ID
    [HttpGet("{id}")]
    public ActionResult<Participant> GetParticipant(int id)
    {
        var participant = _context.Participants
            .Include(p => p.Trainings)
            .FirstOrDefault(p => p.Id == id);

        if (participant == null)
        {
            return NotFound();
        }

        return Ok(participant);
    }



    [HttpGet("{id}/Trainings")]
    public IActionResult GetParticipantTrainings(int id)
    {
        var participant = _context.Participants
                                  .Include(p => p.Trainings)
                                  .FirstOrDefault(p => p.Id == id);

        if (participant == null)
            return NotFound("Participant not found");

        var trainings = participant.Trainings.Select(t => new
        {
            t.Id,
            t.Name,
            t.Description,
            t.StartDate,
            t.EndDate
        }).ToList();

        return Ok(trainings);
    }

    [HttpGet("ParticipantsWithTrainings")]
    public IActionResult GetAllParticipantsWithTrainings()
    {
        var participants = _context.Participants
            .Include(p => p.Trainings)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Email,
                Trainings = p.Trainings.Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.StartDate,
                    t.EndDate
                }).ToList()
            }).ToList();

        return Ok(participants);
    }



    // Добавить участника
    [HttpPost]
    public async Task<IActionResult> CreateParticipant(ParticipantDto participantDto)
    {
        // Получаем тренировку по ее ID
        var training = await _context.Trainings.Include(t => t.Participants)
                                               .FirstOrDefaultAsync(t => t.Id == participantDto.Trainings.FirstOrDefault());

        if (training == null)
        {
            return NotFound("Training not found.");
        }

        // Получаем участника (если уже есть такой)
        var existingParticipant = await _context.Participants
            .Include(p => p.Trainings)
            .FirstOrDefaultAsync(p => p.Email == participantDto.Email);

        if (existingParticipant != null)
        {
            // Проверка на пересечение времени
            foreach (var existingTraining in existingParticipant.Trainings)
            {
                if (training.StartDate < existingTraining.EndDate && training.EndDate > existingTraining.StartDate)
                {
                    return BadRequest("The participant is already registered for a training at this time.");
                }
            }
        }

        // Проверяем, можно ли добавить участника на тренировку
        if (training.Participants.Count >= training.MaxParticipants)
        {
            return BadRequest("Cannot add participant. The training is already full.");
        }

        // Создаем участника
        var participant = new Participant
        {
            Name = participantDto.Name,
            Username = participantDto.Username,
            PasswordHash = Participant.GetHash(participantDto.Password),
            Role = participantDto.Role,
            Email = participantDto.Email,
            Phone = participantDto.Phone,
            Trainings = await _context.Trainings
                .Where(t => participantDto.Trainings.Contains(t.Id))
                .ToListAsync()
        };

        // Добавляем участника в контекст
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetParticipant), new { id = participant.Id }, participant);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParticipant(int id, [FromBody] ParticipantDto participantDto)
    {
        if (participantDto == null)
            return BadRequest("No data provided.");

        var participant = await _context.Participants
                                         .Include(p => p.Trainings)
                                         .FirstOrDefaultAsync(p => p.Id == id);

        if (participant == null)
            return NotFound("Participant not found.");

        // Обновляем только переданные поля
        if (!string.IsNullOrEmpty(participantDto.Username))
            participant.Username = participantDto.Username;

        if (!string.IsNullOrEmpty(participantDto.Name))
            participant.Name = participantDto.Name;

        if (!string.IsNullOrEmpty(participantDto.Email))
            participant.Email = participantDto.Email;

        if (!string.IsNullOrEmpty(participantDto.Role))
            participant.Role = participantDto.Role;

        if (!string.IsNullOrEmpty(participantDto.Phone))
            participant.Phone = participantDto.Phone;

        if (participantDto.Trainings != null && participantDto.Trainings.Any())
        {
            var newTrainings = await _context.Trainings
                                             .Where(t => participantDto.Trainings.Contains(t.Id))
                                             .ToListAsync();
            participant.Trainings = newTrainings;
        }

        // Сохраняем изменения
        _context.Participants.Update(participant);
        await _context.SaveChangesAsync();

        return Ok(participant);
    }




    // Удалить участника
    [HttpDelete("{id}")]
    public IActionResult DeleteParticipant(int id)
    {
        var participant = _context.Participants.Find(id);

        if (participant == null)
        {
            return NotFound();
        }

        _context.Participants.Remove(participant);
        _context.SaveChanges();

        return NoContent();
    }
}
