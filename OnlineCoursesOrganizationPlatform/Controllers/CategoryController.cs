using Microsoft.AspNetCore.Mvc;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ITokenService _tokenService;
        private readonly IActionService _actionService;

        public CategoryController(CategoryService categoryService, ITokenService tokenService, IActionService actionService)
        {
            _categoryService = categoryService;
            _tokenService = tokenService;
            _actionService = actionService;
        }

        /// <summary>
        /// Получение всех категорий
        /// </summary>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpGet("get-all-categories")]
        public IActionResult GetAllCategories()
        {
            IEnumerable<Category> categories = _categoryService.GetAllElements();
            if (categories == null)
            {
                return NotFound("Категории не найдены");
            }
            return Ok(categories);
        }

        /// <summary>
        /// Получение всех активных категорий
        /// </summary>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpGet("get-all-active-categories")]
        public IActionResult GetAllActiveCategories()
        {
            IEnumerable<Category> categories = _categoryService.GetAllActiveElements();
            if (categories == null)
            {
                return NotFound("Категории не найдены");
            }
            return Ok(categories);
        }

        /// <summary>
        /// Получение категории по идентификатору
        /// </summary>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpGet("get-category-by-id")]
        public IActionResult GetCategoryById(int id)
        {
            if (id < 0)
            {
                return BadRequest("Введите корректный индекс");
            }
            Category category = _categoryService.GetElementById(id);
            if (category == null)
            {
                return NotFound("Категория не найдены");
            }
            return Ok(category);
        }

        /// <summary>
        /// Получение всех категорий включающих в себя имя
        /// </summary>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpGet("get-categories-by-name")]
        public IActionResult GetCategoriesByName(string categoryName)
        {
            IEnumerable<Category> category = _categoryService.GetAllElementsByName(categoryName);
            if (category == null)
            {
                return NotFound("Категория не найдены");
            }
            return Ok(category);
        }

        /// <summary>
        /// Получение всех активных категорий включающих в себя имя
        /// </summary>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpGet("get-active-categories-by-name")]
        public IActionResult GetActiveCategoriesByName(string categoryName)
        {
            IEnumerable<Category> category = _categoryService.GetAllActiveElementsByName(categoryName);
            if (category == null)
            {
                return NotFound("Категория не найдены");
            }
            return Ok(category);
        }

        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="CategoryAddRequest">Пользователь</param>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpPost("create-category")]
        public IActionResult AddCategory(CategoryAddRequest categoryAddRequest)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            int categoryId = _categoryService.AddElement(categoryAddRequest, userId);

            _actionService.LogAction("create", "category", categoryId, userId);

            return Ok($"Категория {categoryAddRequest.CategoryName} успешно добавлена");
        }

        /// <summary>
        /// Редактирование категории
        /// </summary>
        /// <param name="CategoryAddRequest">Пользователь</param>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpPut("edit-category")]
        public IActionResult UpdateCategory(int id, CategoryAddRequest categoryAddRequest)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            var category = _categoryService.GetElementById(id);
            if (category == null)
            {
                return NotFound("Категория с таким индексом не найдена");
            }

            _categoryService.UpdateElement(id, categoryAddRequest, userId);
            _actionService.LogAction("update", "category", category.CategoryId, userId);
            return Ok("Category updated successfully");
        }

        /// <summary>
        /// Удаление категории по идентификатору
        /// </summary>
        /// <returns></returns>

        // POST api/<UserController>
        [HttpDelete("delete-category")]
        public IActionResult DeleteCategory(int id)
        {
            // Проверка наличия токена
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                return Ok("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            var category = _categoryService.GetElementById(id);
            if (category == null)
            {
                return NotFound("Категория с таким индексом не найдена");
            }

            _categoryService.DeleteElement(id, userId);
            _actionService.LogAction("delete", "category", category.CategoryId, userId);
            return Ok("Категория удалена");
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