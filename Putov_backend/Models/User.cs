
using System.Security.Cryptography;
using System.Text;

namespace Putov_backend.Models
{
    public class User
    {
        public int Id { get; set; } // Уникальный идентификатор
        public string Username { get; set; } // Логин
        public string PasswordHash { get; set; } // Хэш пароля
        public string Role { get; set; } // Роль (Admin, User и т.д.)


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
