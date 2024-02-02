using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineCoursesOrganizationPlatform.Models;
//using OnlineCoursesOrganizationPlatform.Data;
using System;
using System.Linq;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("UserRegistration")]
        public IActionResult UserRegistration([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                // Проверяем, есть ли уже пользователь с таким email
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    return Conflict("Пользователь с таким email уже зарегистрирован!");
                }

                // Добавляем дату создания
                user.CreatedAt = DateTime.UtcNow;

                // Сохраняем нового пользователя
                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok($"Вы успешно зарегистрированы! Добро пожаловать,{user.FirstName} {user.LastName}!");
            }
            else
            {
                return BadRequest("Некорректные данные пользователя");
            }
        }
    }
}