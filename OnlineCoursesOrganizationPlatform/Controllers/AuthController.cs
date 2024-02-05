using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;


namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly IActionService _actionService;

        public AuthController(ITokenService tokenService, IUserService userService, IJwtService jwtService, ApplicationDbContext context, IActionService actionService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _jwtService = jwtService;
            _context = context;
            _actionService = actionService;
        }

        // Login для генерации токена
        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            if (!string.IsNullOrEmpty(_tokenService.Token))
            {
                // Если токен уже есть, возвращаем соответствующее сообщение
                return Ok("Вы уже в системе!");
            }
            // Ваша логика аутентификации
            var user = _userService.Authenticate(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized("Неверные учетные данные");
            }

            // Генерация токена доступа
            var tokenString = _jwtService.GenerateToken(user);

            // Сохранение токена в сервисе
            _tokenService.Token = tokenString;

            // Добавляем запись о регистрации в систему действий
            _actionService.LogAction("login", "user", user.UserId, user.UserId);

            // Возврат токена доступа клиенту
            return Ok($"Добро пожаловать! Ваш токен успешно сохранен в системе! \nВаш токен:{tokenString}");
        }

        // Login для удаления токена
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            // Получаем токен из заголовка запроса
            string token = _tokenService.Token;

            // Расшифровываем токен, чтобы получить информацию о пользователе
            var userId = ExtractUserIdFromToken(token);

            // Удаление токена из сервиса при выходе
            _tokenService.Token = null;

            // Добавляем запись о регистрации в систему действий
            _actionService.LogAction("logout", "user", userId, userId);

            return Ok("Вы успешно вышли из системы");
        }

        // извлечение Айди пользователя из токена
        [HttpGet("extract-user-id")]
        public int ExtractUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            // Если айди не найден или не может быть преобразован, вернуть значение по умолчанию или выбросить исключение.
            throw new InvalidOperationException("Unable to extract user id from token.");
        }
    }
}

