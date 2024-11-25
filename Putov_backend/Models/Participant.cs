namespace Putov_backend.Models
{
    public class Participant
    {
        public int Id { get; set; } // Уникальный идентификатор участника
        public string Name { get; set; } // Имя участника
        public string Email { get; set; } // Контактный email участника
        public string Phone { get; set; } // Номер телефона

        // Навигационное свойство для связи с тренировками
        public ICollection<Training> Trainings { get; set; }
    }
}
