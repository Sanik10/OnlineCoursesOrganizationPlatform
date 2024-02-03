using System;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(ITokenService tokenService, IUserService userService, IJwtService jwtService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Проверяем наличие токена
            if (!string.IsNullOrEmpty(_tokenService.Token))
            {
                // Если токен уже есть, возвращаем соответствующее сообщение
                return Ok("Вы уже в системе!");
            }

            // Ваша логика аутентификации
            var user = _userService.Authenticate(loginRequest.Username, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized("Неверные учетные данные");
            }

            // Генерация токена доступа
            var tokenString = _jwtService.GenerateToken(user);

            // Сохранение токена в сервисе
            _tokenService.Token = tokenString;

            // Возврат токена доступа клиенту
            return Ok(new { Token = tokenString });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Удаление токена из сервиса при выходе
            _tokenService.Token = null;

            return Ok("Вы успешно вышли из системы");
        }
    }
}

