using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Putov_backend.Data;
using Putov_backend.Models;

//контроллер для обработки CRUD операций с объектами модели Training

[ApiController]
[Route("api/[controller]")] //https://localhost:7242/api/Training
public class TrainingController : ControllerBase //Теперь TrCon работает как API контроллер
    {
        private readonly AppDbContext _context;

    public TrainingController(AppDbContext context)
    {
        _context = context;
    }

    // Атрибут, указывающий, что это метод для обработки GET-запросов.
    [HttpGet]
    public ActionResult<IEnumerable<object>> GetTrainingsWithParticipants()
    {
        
        var trainingsWithParticipants = _context.Trainings
            .Include(t => t.Participants)  // Загружаем участников для каждой тренировки
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.MaxParticipants,
                ParticipantsCount = t.Participants.Count  // Считаем количество участников
            })
            .ToList();

        return Ok(trainingsWithParticipants);
    }

    //ВЫВОД ТРЕНИРОВКИ И ВСЕХ ИМЁН ЛЮДЕЙ
    /*    var trainingsWithParticipants = _context.Trainings
        .Include(t => t.Participants) // Включаем участников для каждой тренировки
        .Select(t => new
        {
            t.Id,
            t.Name,
            Participants = t.Participants.Select(p => new
            {
                p.Name,
                p.Email
            }).ToList()
        })
        .ToList();*/

    [HttpGet("{id}/Participants")]
    public IActionResult GetParticipantsByTrainingId(int id)
    {
        var training = _context.Trainings
            .Include(t => t.Participants) // Подключаем участников через Include
            .FirstOrDefault(t => t.Id == id);

        if (training == null)
        {
            return NotFound($"Training with ID {id} not found.");
        }

        var participants = training.Participants.Select(p => new
        {
            p.Id,
            p.Name,
            p.Email,
            p.Phone
        }).ToList();

        return Ok(participants);
    }



    [HttpGet("{id}/Summary")]
    public IActionResult GetTrainingSummary(int id)
    {
        var training = _context.Trainings
            .Include(t => t.Participants)
            .FirstOrDefault(t => t.Id == id);

        if (training == null)
        {
            return NotFound($"Training with ID {id} not found.");
        }

        var summary = new
        {
            training.Id,
            training.Name,
            training.Description,
            training.StartDate,
            training.EndDate,
            ParticipantsCount = training.Participants.Count
        };

        return Ok(summary);
    }

    [HttpGet("TrainingsWithParticipants")]
    public IActionResult GetAllTrainingsWithParticipants()
    {
        var trainings = _context.Trainings
            .Include(t => t.Participants)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                Participants = t.Participants.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Email
                }).ToList()
            }).ToList();

        return Ok(trainings);
    }

    [HttpGet("training-analytics")]
    public IActionResult GetTrainingAnalytics()
    {
        var trainingStats = _context.Trainings
            .Select(t => new
            {
                TrainingName = t.Name,
                ParticipantsCount = t.Participants.Count,
                MaxParticipants = t.MaxParticipants,
                FillRate = ((double)t.Participants.Count / t.MaxParticipants) * 100
            })
            .OrderBy(t => t.FillRate) // Сортировка по заполненности
            .ToList();

        return Ok(trainingStats);
    }





    [HttpGet("training-analysis")]
    public IActionResult GetTrainingAnalysis(double threshold = 30.0)
    {
        // Анализ тренировок
        var trainingAnalysis = _context.Trainings
            .Select(t => new
            {
                TrainingName = t.Name,
                FillRate = (double)t.Participants.Count / t.MaxParticipants * 100,
                MissingCount = t.MaxParticipants - t.Participants.Count,
                MaxParticipants = t.MaxParticipants,
                CurrentParticipants = t.Participants.Count
            })
            .OrderByDescending(t => t.MissingCount) // Сортировка по недостающим местам
            .ToList();

        // Рекомендации
        var recommendations = trainingAnalysis.Select(t => new
        {
            t.TrainingName,
            t.FillRate,
            Recommendation = t.FillRate switch
            {
                < 30 => "Рассмотрите снижение цены или изменение времени проведения.",
                >= 30 and < 70 => "Попробуйте улучшить условия, например, предложить бонусы.",
                >= 70 => "Популярная тренировка. Можно увеличить количество слотов или создать дополнительную тренировку.",
                _ => "Нет рекомендаций."
            }
        }).ToList();

        // Сгруппировать по популярности
        var categorizedTrainings = new
        {
            Popular = trainingAnalysis.Where(t => t.FillRate >= 70).ToList(),
            Medium = trainingAnalysis.Where(t => t.FillRate >= 30 && t.FillRate < 70).ToList(),
            Low = trainingAnalysis.Where(t => t.FillRate < 30).ToList(),
            Recommendations = recommendations
        };

        return Ok(categorizedTrainings);
    }







    [Authorize(Roles = "admin")]
    [HttpPost] // Атрибут для обработки POST-запросов.
    public IActionResult CreateTraining([FromBody] Training training) //IActionResult — тип возвращаемого значения, который позволяет указать, какой HTTP-ответ отправить (например, успех, ошибка, перенаправление).
    {
        _context.Trainings.Add(training); //добавляет в контекст
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetTrainingsWithParticipants), new { id = training.Id }, training); // Возвращает ответ с созданным объектом.
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id}")] // Атрибут для обработки PUT-запросов (обновление данных по id).
    public IActionResult UpdateTraining(int id, [FromBody] Training training) //Атрибут [FromBody] указывает, что данные будут переданы в формате JSON в теле HTTP-запроса и должны быть преобразованы в объект типа Training.
    {
        var existingTraining = _context.Trainings.Find(id);
        if (existingTraining == null)
            return NotFound();

        // Проверяем и обновляем только те поля, которые были переданы
        if (!string.IsNullOrEmpty(training.Name))
            existingTraining.Name = training.Name;

        if (!string.IsNullOrEmpty(training.Description))
            existingTraining.Description = training.Description;

        if (training.MaxParticipants > 0)
            existingTraining.MaxParticipants = training.MaxParticipants;

        if (training.StartDate != default)
            existingTraining.StartDate = training.StartDate;

        if (training.EndDate != default)
            existingTraining.EndDate = training.EndDate;

        _context.SaveChanges(); // Сохраняем изменения в базе
        return NoContent(); // Успешное обновление
    }
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")] //Атрибут для обработки DELETE-запросов (удаление)
    public IActionResult DeleteTraining(int id)
    {
        var training = _context.Trainings.Find(id);
        if (training == null) // Если тренировка не найдена, возвращает 404.
            return NotFound();
        _context.Trainings.Remove(training); // Удаляет тренировку из контекста.
        _context.SaveChanges(); // Сохраняет изменения.
        return NoContent();
    }

    [HttpGet("{id}")]
    public ActionResult<object> GetTraining(int id) //В данном случае возвращаемое значение будет объектом любого типа
    {
        var training = _context.Trainings.FirstOrDefault(t => t.Id == id); //LINQ
        if (training == null)
        {
            return NotFound();
        }

        string? duration = null;
        try
        {
            var timeSpan = training.GetDuration();
            duration = $"{timeSpan.Hours} hours, {timeSpan.Minutes} minutes";
        }
        catch (InvalidOperationException ex)
        {
            duration = ex.Message; // Показывает причину ошибки
        }

        return Ok(new
        {
            training.Id,
            training.Name,
            training.Description,
            training.StartDate,
            training.EndDate,
            Duration = duration
        });
    }



}
