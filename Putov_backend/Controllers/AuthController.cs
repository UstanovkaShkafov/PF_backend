using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Putov_backend.Data;
using Putov_backend.Models;


namespace Putov_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public struct LoginData
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        [HttpPost("login")]
        public IActionResult GetToken([FromBody] LoginData ld)
        {
            // Хэшируем пароль для проверки
            var hashedPassword = Models.Participant.GetHash(ld.Password);

            // Ищем пользователя в базе данных
            var user = _context.Participants.FirstOrDefault(u =>
                u.Username == ld.Username && u.PasswordHash == hashedPassword);

            if (user == null)
            {
                // Если пользователь не найден, возвращаем 401 Unauthorized
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Генерируем токен для пользователя
            var token = AuthOptions.GenerateToken(user.Username, user.IsAdmin);

            // Возвращаем токен
            return Ok(token);
        }
    }
}