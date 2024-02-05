using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;

namespace OnlineCoursesOrganizationPlatform.Controllers
{
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        private readonly CourseMaterialService _courseMaterialService;
        private readonly ITokenService _tokenService;
        private readonly IActionService _actionService;

        public MaterialController(CourseMaterialService courseMaterialService, ITokenService tokenService, IActionService actionService)
        {
            _courseMaterialService = courseMaterialService;
            _tokenService = tokenService;
            _actionService = actionService;
        }

        [HttpGet("get-all-materials")]
        public IActionResult GetAllMaterials()
        {
            IEnumerable<CourseMaterial> materials = _courseMaterialService.GetAllElements();
            if (materials == null)
            {
                return NotFound("Материалы не найдены");
            }
            return Ok(materials);
        }

        [HttpGet("get-all-active-materials")]
        public IActionResult GetAllActiveMaterials()
        {
            IEnumerable<CourseMaterial> materials = _courseMaterialService.GetAllActiveElements();
            if (materials == null)
            {
                return NotFound("Материалы не найдены");
            }
            return Ok(materials);
        }

        [HttpGet("get-all-materials-by-name")]
        public IActionResult GetAllMaterialsByName(string materialName)
        {
            IEnumerable<CourseMaterial> materials = _courseMaterialService.GetAllElementsByName(materialName);
            if (materials == null)
            {
                return NotFound("Материалы не найдены");
            }
            return Ok(materials);
        }

        [HttpGet("get-all-active-materials-by-name")]
        public IActionResult GetAllActiveMaterialsByName(string materialName)
        {
            IEnumerable<CourseMaterial> materials = _courseMaterialService.GetAllActiveElementsByName(materialName);
            if (materials == null)
            {
                return NotFound("Материалы не найдены");
            }
            return Ok(materials);
        }

        [HttpGet("get-all-materials-by-course-id")]
        public IActionResult GetMaterialsByCourseId(int courseId)
        {
            if (courseId < 0)
            {
                return BadRequest("Введите корректный индекс");
            }
            IEnumerable<CourseMaterial> materials = _courseMaterialService.GetAllElementsByCourseId(courseId);
            if (materials == null)
            {
                return NotFound("Материалы не найдены");
            }
            return Ok(materials);
        }

        [HttpGet("get-all-active-materials-by-course-id")]
        public IActionResult GetAllActiveMaterialsByCourseId(int courseId)
        {
            if (courseId < 0)
            {
                return BadRequest("Введите корректный индекс");
            }
            IEnumerable<CourseMaterial> materials = _courseMaterialService.GetAllActiveElementsByCourseId(courseId);
            if (materials == null)
            {
                return NotFound("Материалы не найдены");
            }
            return Ok(materials);
        }

        [HttpGet("get-material-by-id")]
        public IActionResult GetMaterialById(int materialId)
        {
            if (materialId < 0)
            {
                return BadRequest("Введите корректный индекс");
            }
            var material = _courseMaterialService.GetElementById(materialId);
            if (material == null)
            {
                return NotFound("Материала под таким номером не найдено");
            }
            return Ok(material);
        }

        [HttpPost("create-material")]
        public IActionResult AddCourseMaterial(CourseMaterialRequest materialRequest)
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                throw new UnauthorizedAccessException("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            int materialId = _courseMaterialService.AddElement(materialRequest, userId);
            _actionService.LogAction("create", "course-material", materialId, userId);
            return Ok("Материал успешно добавлен");
        }

        [HttpPut("edit-material")]
        public IActionResult UpdateCourseMaterial(CourseMaterialUpdateRequest materialRequest)
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                throw new UnauthorizedAccessException("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            _courseMaterialService.UpdateElement(materialRequest.MaterialId, materialRequest, userId);
            _actionService.LogAction("update", "course-material", materialRequest.MaterialId, userId);
            return Ok("Материал успешно обновлен");
        }

        [HttpDelete("delete-material")]
        public IActionResult DeleteCourseMaterial(int materialId)
        {
            if (string.IsNullOrEmpty(_tokenService.Token))
            {
                throw new UnauthorizedAccessException("Для начала войдите или зарегистрируйтесь");
            }
            int userId = this.ExtractUserIdFromToken(_tokenService.Token);

            _courseMaterialService.DeleteElement(materialId, userId);
            _actionService.LogAction("delete", "course-material", materialId, userId);

            return Ok("Материал успешно удален");
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