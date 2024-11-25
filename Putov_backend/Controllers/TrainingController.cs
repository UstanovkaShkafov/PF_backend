using Microsoft.AspNetCore.Mvc;
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

    [HttpGet] // Атрибут, указывающий, что это метод для обработки GET-запросов.
    public IEnumerable<Training> GetTrainings() // Метод для получения списка тренировок.
    {
        return _context.Trainings.ToList();
    }

    [HttpPost] // Атрибут для обработки POST-запросов.
    public IActionResult CreateTraining([FromBody] Training training) //IActionResult — тип возвращаемого значения, который позволяет указать, какой HTTP-ответ отправить (например, успех, ошибка, перенаправление).
    {
        _context.Trainings.Add(training); //добавляет в контекст
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetTrainings), new { id = training.Id }, training); // Возвращает ответ с созданным объектом.
    }

    [HttpPut("{id}")] // Атрибут для обработки PUT-запросов (обновление данных по id).
    public IActionResult UpdateTraining(int id, [FromBody] Training training) //Атрибут [FromBody] указывает, что данные будут переданы в формате JSON в теле HTTP-запроса и должны быть преобразованы в объект типа Training.
    {
        var existingTraining = _context.Trainings.Find(id); // Ищет тренировку по id
        if (existingTraining == null) // Если тренировка не найдена, возвращает 404
            return NotFound();
        existingTraining.Name = training.Name; // Обновление имени тренировки.
        existingTraining.Description = training.Description; // Обновление описания тренировки.
        _context.SaveChanges(); // Сохраняет изменения.
        return NoContent(); // Возвращает статус 204 (успешное обновление без тела ответа)
    }
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
