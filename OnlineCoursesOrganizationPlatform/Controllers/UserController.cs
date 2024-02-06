using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ITokenService _tokenService;
        private readonly IActionService _actionService;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context, IJwtService jwtService, ITokenService tokenService, IActionService actionService, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _tokenService = tokenService;
            _actionService = actionService;
            _jwtService = jwtService;
            _userService = userService;
        }

        /// <summary>
        /// Регистрация в системе
        /// </summary>
        /// <param name="UserRegistrationRequest">Пользователь</param>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpPost("registration")]
        public IActionResult UserRegistration(UserRegistrationRequest userRegistrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Некорректные данные пользователя");
            }

            // Проверяем, существует ли уже пользователь с таким email
            var existingUser = _context.Users.FirstOrDefault(u => u.Email.ToLower() == userRegistrationRequest.Email.ToLower());
            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким email уже зарегистрирован");
            }

            var newUser = new User
            {
                FirstName = userRegistrationRequest.FirstName,
                LastName = userRegistrationRequest.LastName,
                Email = userRegistrationRequest.Email,
                Password = userRegistrationRequest.Password,
                CreatedAt = DateTime.UtcNow
            };

            // Если указан пригласивший пользователь, устанавливаем его айдишник
            if (userRegistrationRequest.CreatedByUserId != null)
            {
                newUser.CreatedByUserId = userRegistrationRequest.CreatedByUserId;
            }

            _context.Users.Add(newUser);
            _context.SaveChanges();

            var tokenString = _jwtService.GenerateToken(newUser);
            _tokenService.Token = tokenString;

            _actionService.LogAction("registration", "user", newUser.UserId, newUser.UserId);

            return Ok($"Вы успешно зарегистрированы! Добро пожаловать, {newUser.FirstName} {newUser.LastName}!");
        }

        /// <summary>
        /// Редактирование аккаунта пользователя
        /// </summary>
        /// <param name="UserUpdateRequest">Пользователь</param>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpPut("edit-data")]
        public IActionResult EditUser(UserUpdateRequest userUpdateRequest)
        {
            int userId = ExtractUserIdFromToken(_tokenService.Token);
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (existingUser == null)
            {
                return NotFound("Пользователь не найден");
            }

            if (existingUser.Password != userUpdateRequest.OldPassword)
            {
                return BadRequest("Старый пароль не совпадает");
            }

            if (existingUser.Password == userUpdateRequest.NewPassword)
            {
                return BadRequest("Новый пароль должен отличаться от старого!");
            }

            if (userUpdateRequest.FirstName != null)
                existingUser.FirstName = userUpdateRequest.FirstName;
            if (userUpdateRequest.LastName != null)
                existingUser.LastName = userUpdateRequest.LastName;
            if (userUpdateRequest.Email != null)
                existingUser.Email = userUpdateRequest.Email;
            existingUser.Password = userUpdateRequest.NewPassword;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            _actionService.LogAction("update", "user", userId, userId);

            return Ok($"Данные пользователя {existingUser.FirstName} {existingUser.LastName} успешно обновлены");
        }

        /// <summary>
        /// Удаление аккаунта пользователя
        /// </summary>
        /// <param name="UserDeleteRequest">Пользователь</param>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpDelete("delete")]
        public IActionResult DeleteUser(UserDeleteRequest userDeleteRequest)
        {
            int userId = ExtractUserIdFromToken(_tokenService.Token);
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (existingUser == null)
            {
                return NotFound("Пользователь не найден");
            }

            if (existingUser.Password != userDeleteRequest.Password || existingUser.Password != userDeleteRequest.ConfirmPassword)
            {
                return BadRequest("Пароли не совпадают");
            }

            existingUser.DeletedAt = DateTime.UtcNow;

            _context.SaveChanges();

            _actionService.LogAction("delete", "user", userId, userId);

            return Ok($"Пользователь {existingUser.FirstName} {existingUser.LastName} успешно удален");
        }

        private int ExtractUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            throw new InvalidOperationException("Unable to extract user id from token.");
        }
    }
}