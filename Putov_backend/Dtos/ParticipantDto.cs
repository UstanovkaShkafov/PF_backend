namespace Putov_backend.Dtos
{
    public class ParticipantDto
    {
        public string? Name { get; set; }
        public string? Username { get; set; } // Логин
        public string? Password { get; set; } // Хэш пароля
        public string? Role { get; set; } // Роль (Admin, User и т.д.)
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<int>? Trainings { get; set; }
    }
}
