using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;

public class MaterialController : ControllerBase
{
    private readonly ICourseMaterialService _courseMaterialService;
    private readonly ITokenService _tokenService;
    private readonly IActionService _actionService;

    public MaterialController(ICourseMaterialService courseMaterialService, ITokenService tokenService, IActionService actionService)
    {
        _courseMaterialService = courseMaterialService;
        _tokenService = tokenService;
        _actionService = actionService;
    }

    [HttpGet("{courseId}")]
    public IActionResult GetMaterialsByCourseId(int courseId)
    {
        var materials = _courseMaterialService.GetAllMaterialsByCourseId(courseId);
        return Ok(materials);
    }

    [HttpGet("material/{materialId}")]
    public IActionResult GetMaterialById(int materialId)
    {
        var material = _courseMaterialService.GetMaterialById(materialId);
        if (material == null)
        {
            return NotFound();
        }
        return Ok(material);
    }

    [HttpPost("AddMaterial")]
    public IActionResult AddCourseMaterial(CourseMaterialRequest materialRequest)
    {
        if (string.IsNullOrEmpty(_tokenService.Token))
        {
            throw new UnauthorizedAccessException("Для начала войдите или зарегистрируйтесь");
        }

        int userId = this.ExtractUserIdFromToken(_tokenService.Token);

        var material = new CourseMaterial
        {
            CourseId = materialRequest.CourseId,
            MaterialName = materialRequest.MaterialName,
            MaterialType = materialRequest.MaterialType,
            FilePath = materialRequest.FilePath,
        };

        _courseMaterialService.AddMaterial(material, userId);
        _actionService.LogAction("create", "course-material", material.CourseId, userId);
        return Ok("Материал успешно добавлен");
    }

    [HttpPost("UpdateMaterial")]
    public IActionResult UpdateCourseMaterial(CourseMaterialRequest materialRequest)
    {
        int userId = this.ExtractUserIdFromToken(_tokenService.Token);

        var material = new CourseMaterial
        {
            CourseId = materialRequest.CourseId,
            MaterialName = materialRequest.MaterialName,
            MaterialType = materialRequest.MaterialType,
            FilePath = materialRequest.FilePath,
            UpdatedByUserId = userId,
            UpdatedAt = DateTime.UtcNow
        };

        _courseMaterialService.UpdateMaterial(material, userId);
        _actionService.LogAction("update", "course-material", material.CourseId, userId);
        return Ok("Материал успешно обновлен");
    }

    [HttpPost("DeleteMaterial/{materialId}")]
    public IActionResult DeleteCourseMaterial(int materialId)
    {
        if (string.IsNullOrEmpty(_tokenService.Token))
        {
            throw new UnauthorizedAccessException("Для начала войдите или зарегистрируйтесь");
        }

        int userId = this.ExtractUserIdFromToken(_tokenService.Token);

        _courseMaterialService.DeleteMaterial(materialId, userId);

        // Фиксация действия в журнал
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