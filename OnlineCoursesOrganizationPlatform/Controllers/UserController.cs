﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [ApiController]
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

        [HttpPost("Registration")]
        public IActionResult UserRegistration([FromBody] UserRegistrationRequest userRegistrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Некорректные данные пользователя");
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

        [HttpPost("EditData")]
        public IActionResult EditUser([FromBody] UserUpdateRequest userUpdateRequest)
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

            existingUser.FirstName = userUpdateRequest.FirstName;
            existingUser.LastName = userUpdateRequest.LastName;
            existingUser.Password = userUpdateRequest.NewPassword;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            _actionService.LogAction("update", "user", userId, userId);

            return Ok($"Данные пользователя {existingUser.FirstName} {existingUser.LastName} успешно обновлены");
        }

        [HttpPost("Delete")]
        public IActionResult DeleteUser([FromBody] UserDeleteRequest userDeleteRequest)
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