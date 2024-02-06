using Microsoft.AspNetCore.Mvc;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly ITokenService _tokenService;
        private readonly IActionService _actionService;
        private readonly ApplicationDbContext _context;

        public CourseController(CourseService courseService, ITokenService tokenService, ApplicationDbContext context, IActionService actionService)
        {
            _courseService = courseService;
            _tokenService = tokenService;
            _context = context;
            _actionService = actionService;
        }

        /// <summary>
        /// Получение всех курсов
        /// </summary>
        /// <returns></returns>

        // GET api/<CourseController>
        [HttpGet("get-all-courses")]
        public IActionResult GetAllCourses()
        {
            IEnumerable<Course> course = _courseService.GetAllElements();
            if (course == null)
            {
                return NotFound("Курсов не найдено");
            }
            return Ok(course);
        }

        /// <summary>
        /// Получение всех активных курсов
        /// </summary>
        /// <returns></returns>

        // GET api/<CourseController>
        [HttpGet("get-all-active-courses")]
        public IActionResult GetAllActiveCourses()
        {
            IEnumerable<Course> courses = _courseService.GetAllActiveElements();
            if (courses == null)
            {
                return NotFound("Курсов не найдено");
            }
            return Ok(courses);
        }

        /// <summary>
        /// Получение всех курсов по имени
        /// </summary>
        /// <returns></returns>

        // GET api/<CourseController>
        [HttpGet("get-all-courses-by-name")]
        public IActionResult GetAllCoursesByName(string elementName)
        {
            IEnumerable<Course> courses = _courseService.GetAllElementsByName(elementName);
            if (courses == null)
            {
                return NotFound("Курсов не найдено");
            }
            return Ok(courses);
        }

        /// <summary>
        /// Получение всех активных курсов по имени
        /// </summary>
        /// <returns></returns>

        // GET api/<CourseController>
        [HttpGet("get-all-active-courses-by-name")]
        public IActionResult GetAllActiveCoursesByName(string elementName)
        {
            IEnumerable<Course> courses = _courseService.GetAllActiveElementsByName(elementName);
            if (courses == null)
            {
                return NotFound("Курсов не найдено");
            }
            return Ok(courses);
        }

        /// <summary>
        /// Получение всех курсов по индексу
        /// </summary>
        /// <returns></returns>

        // GET api/<CourseController>
        [HttpGet("get-course-by-id")]
        public IActionResult GetCourseById(int id)
        {
            if (id < 0)
            {
                return BadRequest("Введите корректный индекс");
            }
            var course = _courseService.GetElementById(id);
            if (course == null)
            {
                return NotFound("Курс с указанным идентификатором не найден.");
            }
            return Ok(course);
        }

        /// <summary>
        /// Создание курса
        /// </summary>
        /// <param name="CourseDto">Модель запроса на создание курса</param>
        /// <returns></returns>

        // POST api/<CourseController>
        [HttpPost("create-course")]
        public IActionResult AddCourse(CourseDto courseDto)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            if (courseDto.CategoryId == null)
            {
                return BadRequest("Категория курса должна быть указана");
            }

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == courseDto.CategoryId);
            if (category == null)
            {
                return BadRequest("Указан неправильный идентификатор категории. \nПодсказка: на вкладке Category вы можете узнать все доступные категории \nнажав на кнопку get-all-categories");
            }

            int courseId = _courseService.AddElement(courseDto, userId);

            _actionService.LogAction("create", "course", courseId, userId);

            return Ok("Курс успешно создан!");
        }

        /// <summary>
        /// Редактирование курса
        /// </summary>
        /// /// <param name="CourseDto">Модель редактирование на создание курса</param>
        /// <returns></returns>

        // PUT api/<CourseController>
        [HttpPut("edit-course")]
        public IActionResult UpdateCourse([Required] int courseId, CourseDto courseDto)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            var category = _courseService.GetElementById(courseId);
            if (category == null)
            {
                return NotFound("Курс для редактирования с таким индексом не найден. \nПодсказка: воспользуйтесь кнопкой get-all-active-courses для поиска нужного вам.");
            }

            _courseService.UpdateElement(courseId, courseDto, userId);

            _actionService.LogAction("update", "course", courseId, userId);

            return Ok("Курс успешно изменён!");
        }

        /// <summary>
        /// Удаление курса
        /// </summary>
        /// <returns></returns>

        // DELETE api/<CourseController>
        [HttpDelete("delete-course")]
        public IActionResult DeleteCourse([Required]int id)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            var existingCourse = _courseService.GetElementById(id);
            if (existingCourse == null)
            {
                return NotFound("Такого курса не существует!");
            }

            _courseService.DeleteElement(id, userId);
            _actionService.LogAction("delete", "course", id, userId);
            return Ok("Курс удален");
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