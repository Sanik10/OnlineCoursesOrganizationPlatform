using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineCoursesOrganizationPlatform.Models;
//using OnlineCoursesOrganizationPlatform.Data;
using System;
using System.Linq;


namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("Registration")]
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
                user.DeletedAt = null;
                user.UpdatedAt = null;
                user.DeletedByUserId = null;

                // Сохраняем нового пользователя
                _context.Users.Add(user);
                _context.SaveChanges();

                // Добавляем запись о регистрации в систему действий
                Actions registrationAction = new Actions
                {
                    ActionType = "registration",
                    EntityType = "user",
                    EntityId = user.UserId,
                    UserId = user.UserId, // ID нового пользователя
                    CreatedAt = DateTime.UtcNow
                };

                _context.Actions.Add(registrationAction);
                _context.SaveChanges();

                return Ok($"Вы успешно зарегистрированы! Добро пожаловать, {user.FirstName} {user.LastName}!");
            }
            else
            {
                return BadRequest("Некорректные данные пользователя");
            }
        }

        [HttpPost("EditData")]
        public IActionResult EditUser([FromBody] UserUpdateRequest userUpdateRequest)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == userUpdateRequest.Email);
                if (existingUser == null)
                {
                    return NotFound("Пользователь не найден");
                }

                // Проверка старого пароля
                if (existingUser.Password != userUpdateRequest.OldPassword)
                {
                    return BadRequest("Старый пароль не совпадает");
                }

                if (existingUser.Password == userUpdateRequest.NewPassword)
                {
                    return BadRequest("Новый пароль должен отличаться от старого!");
                }

                // Обновляем данные пользователя
                existingUser.FirstName = userUpdateRequest.FirstName;
                existingUser.LastName = userUpdateRequest.LastName;
                existingUser.Password = userUpdateRequest.NewPassword; // Обновление пароля

                // Устанавливаем дату обновления
                existingUser.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                // Добавляем запись о изменении в систему действий
                Actions registrationAction = new Actions
                {
                    ActionType = "update",
                    EntityType = "user",
                    EntityId = existingUser.UserId,
                    UserId = existingUser.UserId, // ID нового пользователя
                    CreatedAt = DateTime.UtcNow
                };

                _context.Actions.Add(registrationAction);
                _context.SaveChanges();

                return Ok($"Данные пользователя {existingUser.FirstName} {existingUser.LastName} успешно обновлены");
            }
            else
            {
                return BadRequest("Некорректные данные пользователя");
            }
        }

        [HttpPost("Delete")]
        public IActionResult DeleteUser([FromBody] UserDeleteRequest userDeleteRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Некорректные данные пользователя");
            }

            // Проверка наличия пользователя по указанному email
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == userDeleteRequest.Email);
            if (existingUser == null)
            {
                return NotFound("Пользователь не найден");
            }

            // Проверка совпадения паролей
            if (existingUser.Password != userDeleteRequest.Password || userDeleteRequest.Password != userDeleteRequest.ConfirmPassword)
            {
                return BadRequest("Пароли не совпадают");
            }

            // Удаление пользователя из базы данных
            existingUser.DeletedAt = DateTime.UtcNow; // Помечаем пользователя как удаленного
            _context.SaveChanges();

            // Добавление записи об удалении пользователя в систему действий
            Actions deleteAction = new Actions
            {
                ActionType = "delete",
                EntityType = "user",
                EntityId = existingUser.UserId,
                UserId = existingUser.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Actions.Add(deleteAction);
            _context.SaveChanges();

            return Ok($"Пользователь {existingUser.FirstName} {existingUser.LastName} успешно удален");
        }
    }
}