namespace Putov_backend.Models
{
    public class Training
    {
        public int Id { get; set; } // Уникальный идентификатор
        public string Name { get; set; } 
        public DateTime Data { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TimeSpan GetDuration()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
                throw new InvalidOperationException("Даты начала и окончания должны быть установлены.");
            if (EndDate.Value < StartDate.Value) 
                throw new InvalidOperationException("Дата окончания не может быть раньше даты начала.");
            return EndDate.Value - StartDate.Value;
        }

        public ICollection<Participant> Participants { get; set; } // Участники тренировки
    }

}
