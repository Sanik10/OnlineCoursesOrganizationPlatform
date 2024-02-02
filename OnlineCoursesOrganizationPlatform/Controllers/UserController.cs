using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("UserRegistration")]
        public IActionResult UserRegistration(string fisrtName, string lastName, string email, string password)
        {
            return Ok($"Пользователь успешно зарегистрирован!{fisrtName} {lastName}");
        }
    }
}