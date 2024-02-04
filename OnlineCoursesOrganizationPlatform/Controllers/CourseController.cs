using Microsoft.AspNetCore.Mvc;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ITokenService _tokenService;
        private readonly IActionService _actionService;
        private readonly ApplicationDbContext _context;

        public CourseController(ICourseService courseService, ITokenService tokenService, ApplicationDbContext context, IActionService actionService)
        {
            _courseService = courseService;
            _tokenService = tokenService;
            _context = context;
            _actionService = actionService;
        }

        [HttpGet("all-courses")]
        public IActionResult GetAllCourses()
        {
            var courses = _courseService.GetAllCourses();
            return Ok(courses);
        }

        [HttpGet("all-active-courses")]
        public IActionResult GetAllActiveCourses()
        {
            var courses = _courseService.GetAllActiveCourses();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public IActionResult GetCourseById(int id)
        {
            var course = _courseService.GetCourseById(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpPost("create-course")]
        public IActionResult AddCourse([FromBody] CourseDto courseDto)
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }

            string token = _tokenService.Token;
            var userId = ExtractUserIdFromToken(token);

            var course = new Course
            {
                CourseName = courseDto.CourseName,
                Description = courseDto.Description,
                CreatedByUserId = userId,
                DeletedByUserId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _courseService.CreateCourse(course, userId);

            _actionService.LogAction("create", "course", course.CourseId, userId);

            return Ok("Курс успешно создан!");
        }

        [HttpPut("edit-course/{id}")]
        public IActionResult UpdateCourse(int id, [FromBody] CourseEditRequestDto courseDto)
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            var course = _courseService.GetCourseById(id);
            if (course == null)
            {
                return NotFound("Курса под таким номером не найдено");
            }

            course.CourseName = courseDto.CourseName;
            course.Description = courseDto.Description;
            course.UpdatedAt = DateTime.UtcNow;

            _courseService.UpdateCourse(id, courseDto, ExtractUserIdFromToken(_tokenService.Token));

            _actionService.LogAction("update", "course", id, ExtractUserIdFromToken(_tokenService.Token));

            return Ok("Курс успешно изменён!");
        }

        [HttpDelete("delete-course/{id}")]
        public IActionResult DeleteCourse(int id)
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            var existingCourse = _courseService.GetCourseById(id);
            if (existingCourse == null)
            {
                return NotFound("Такого курса не существует!");
            }

            _courseService.DeleteCourse(id, ExtractUserIdFromToken(_tokenService.Token));
            _actionService.LogAction("delete", "course", id, ExtractUserIdFromToken(_tokenService.Token));
            return Ok("Курс удален");
        }

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

            throw new InvalidOperationException("Unable to extract user id from token.");
        }
    }
}