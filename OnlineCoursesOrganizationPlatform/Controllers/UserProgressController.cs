using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [Route("api/[controller]")]
    public class UserProgressController : ControllerBase
    {
        private readonly UserProgressService _userProgressService;
        private readonly ITokenService _tokenService;

        public UserProgressController(UserProgressService userProgressService, ITokenService tokenService)
        {
            _userProgressService = userProgressService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Изменение прогресса
        /// </summary>
        /// <returns></returns>
        /// <param name="UserProgressUpdateRequest">Модель запроса на изменение прогресса изучения</param>

        // POST api/<UserProgressController>
        [HttpPost("update-progress")]
        public IActionResult UpdateProgress(UserProgressUpdateRequest progressRequest)
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Unauthorized("Для доступа к ресурсам необходимо авторизоваться.");
            }

            int userId = ExtractUserIdFromToken(_tokenService.Token);

            _userProgressService.UpdateProgress(progressRequest, userId);

            return Ok("Прогресс успешно обновлен.");
        }

        /// <summary>
        /// Получение прогресса у пользователя
        /// </summary>
        /// <returns></returns>

        // POST api/<UserProgressController>
        [HttpPost("get-user-progress")]
        public IActionResult GetProgress()
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Unauthorized("Для доступа к ресурсам необходимо авторизоваться.");
            }

            int userId = ExtractUserIdFromToken(_tokenService.Token);

            IEnumerable<UserProgress> userProgresses = _userProgressService.GetProgressByUserId(userId);

            return Ok(userProgresses);
        }

        /// <summary>
        /// Извлечение айди из токена
        /// </summary>
        /// <returns></returns>
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