﻿using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [Route("api/[controller]")]
    public class CourseRatingController : ControllerBase
    {
        private readonly CourseRatingsService _courseRatingsService;
        private readonly ITokenService _tokenService;
        private readonly IActionService _actionService;
        private readonly ApplicationDbContext _context;

        public CourseRatingController(ApplicationDbContext context, ITokenService tokenService, IActionService actionService, CourseRatingsService courseRatingsService)
        {
            _context = context;
            _tokenService = tokenService;
            _actionService = actionService;
            _courseRatingsService = courseRatingsService;
        }

        [HttpGet("get-all-ratings")]
        public IActionResult GetAllRatings()
        {
            IEnumerable<CourseRating> course = _courseRatingsService.GetAllElements();
            if (course == null)
            {
                return NotFound("Оценок не найдено");
            }
            return Ok(course);
        }

        [HttpGet("get-all-active-ratings")]
        public IActionResult GetAllActiveRatings()
        {
            IEnumerable<CourseRating> course = _courseRatingsService.GetAllActiveElements();
            if (course == null)
            {
                return NotFound("Оценок не найдено");
            }
            return Ok(course);
        }

        [HttpGet("get-all-ratings-by-name")]
        public IActionResult GetAllRatingsByName(string ratingName)
        {
            IEnumerable<CourseRating> course = _courseRatingsService.GetAllElementsByName(ratingName);
            if (course == null)
            {
                return NotFound("Оценок не найдено");
            }
            return Ok(course);
        }

        [HttpGet("get-all-active-ratings-by-name")]
        public IActionResult GetAllActiveRatingsByName(string ratingName)
        {
            IEnumerable<CourseRating> course = _courseRatingsService.GetAllElementsByName(ratingName);
            if (course == null)
            {
                return NotFound("Оценок не найдено");
            }
            return Ok(course);
        }

        [HttpGet("get-ratings-by-id")]
        public IActionResult GetAllActiveRatingsByName(int ratingId)
        {
            if (ratingId < 0)
            {
                return BadRequest("Введите корректный индекс");
            }
            CourseRating course = _courseRatingsService.GetElementById(ratingId);
            if (course == null)
            {
                return NotFound("Оценок не найдено");
            }
            return Ok(course);
        }

        [HttpPost("create-rating/{ratingId}")]
        public IActionResult CreateRating(CourseRatingCreateRequest courseRatingCreateRequest)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            int ratingId = _courseRatingsService.AddElement(courseRatingCreateRequest, userId);
            _actionService.LogAction("create", "course-rating", ratingId, userId);
            return Ok("Оценка оставлена!");
        }

        [HttpPut("update-rating/{ratingId}")]
        public IActionResult UpdateRating(int ratingId, CourseRatingUpdateRequest courseRatingUpdateRequest)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }

            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            var ratingExists = _courseRatingsService.CheckIfRatingExists(ratingId);
            if (!ratingExists)
            {
                return NotFound("Оценка не найдена");
            }

            _courseRatingsService.UpdateElement(ratingId, courseRatingUpdateRequest, userId);
            _actionService.LogAction("update", "course-rating", ratingId, userId);
            return Ok("Оценка успешно обновлена!");
        }

        [HttpDelete("delete-rating/{ratingId}")]
        public IActionResult DeleteRating(int ratingId)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }

            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            var ratingExists = _courseRatingsService.CheckIfRatingExists(ratingId);
            if (!ratingExists)
            {
                return NotFound("Оценка не найдена");
            }

            _courseRatingsService.DeleteElement(ratingId, userId);
            _actionService.LogAction("delete", "course-rating", ratingId, userId);
            return Ok("Оценка успешно удалена!");
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
