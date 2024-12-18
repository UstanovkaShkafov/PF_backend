using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Putov_backend.Models
{
    public class Participant
    {
        public int Id { get; set; } // Уникальный идентификатор участника
        public string Username { get; set; } // Логин
        public string PasswordHash { get; set; } // Хэш пароля
        public string Role { get; set; } // Роль (Admin, User и т.д.)
        public string Name { get; set; } // Имя участника
        public string Email { get; set; } // Контактный email участника
        public string Phone { get; set; } // Номер телефона
        public bool IsAdmin => Role == "admin";
        public bool CheckPassword(string password) => password == PasswordHash;

        //public int TrainingId { get; set; }

        // Навигационное свойство для связи с тренировками
        //[JsonIgnore] // ЕСЛИ НЕ НУЖНО ВИДЕТЬ КАЖДУЮ ТРЕНИРОВКУ У КАЖДОГО ЧЕЛОВЕКА
        public ICollection<Training> Trainings { get; set; } = new List<Training>();

        public static string GetHash(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            var sb = new StringBuilder();
            foreach (var b in MD5.Create().ComputeHash(textBytes))
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

    }
}
