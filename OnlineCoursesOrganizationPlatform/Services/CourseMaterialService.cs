using System;
using System.Collections.Generic;
using System.Linq;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public class CourseMaterialService : IService<CourseMaterial, CourseMaterialRequest, CourseMaterialUpdateRequest>
    {
        private readonly ApplicationDbContext _context;

        public CourseMaterialService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получение всех материалов курса
        public IEnumerable<CourseMaterial> GetAllElements()
        {
            return _context.CourseMaterials.ToList();
        }

        // Получение всех активных материалов курса
        public IEnumerable<CourseMaterial> GetAllActiveElements()
        {
            return _context.CourseMaterials.Where(m => m.DeletedAt == null).ToList();
        }

        // Получение всех материалов курса по имени
        public IEnumerable<CourseMaterial> GetAllElementsByName(string elementName)
        {
            return _context.CourseMaterials.Where(m => m.MaterialName.Contains(elementName)).ToList();
        }

        // Получение всех активных материалов курса по имени
        public IEnumerable<CourseMaterial> GetAllActiveElementsByName(string elementName)
        {
            return _context.CourseMaterials.Where(m => m.MaterialName.Contains(elementName) && m.DeletedAt == null).ToList();
        }

        // Получение всех материалов курса по айди курса
        public IEnumerable<CourseMaterial> GetAllElementsByCourseId(int courseId)
        {
            return _context.CourseMaterials.Where(m => m.CourseId == courseId).ToList();
        }

        // Получение всех активных материалов курса по айди курса
        public IEnumerable<CourseMaterial> GetAllActiveElementsByCourseId(int courseId)
        {
            return _context.CourseMaterials.Where(m => m.CourseId == courseId && m.DeletedAt == null).ToList();
        }

        // Получение материала курса по айди
        public CourseMaterial GetElementById(int materialId)
        {
            return _context.CourseMaterials.FirstOrDefault(m => m.MaterialId == materialId && m.DeletedAt == null);
        }

        // Добавлние материала курса
        public int AddElement(CourseMaterialRequest materialRequest, int userId)
        {
            CourseMaterial material = new CourseMaterial
            {
                CourseId = materialRequest.CourseId,
                MaterialNumber = materialRequest.MaterialNumber,
                MaterialName = materialRequest.MaterialName,
                MaterialContent = materialRequest.MaterialContent,
                FilePath = materialRequest.FilePath,
            };
            material.CreatedByUserId = userId;
            material.CreatedAt = DateTime.UtcNow;
            material.UpdatedAt = null;
            material.DeletedAt = null;

            _context.CourseMaterials.Add(material);
            _context.SaveChanges();

            return material.MaterialId;
        }

        // Измение материала курса
        public void UpdateElement(int materialId, CourseMaterialUpdateRequest materialUpdate, int userId)
        {
            CourseMaterial existingMaterial = _context.CourseMaterials.FirstOrDefault(m => m.MaterialId == materialId && m.DeletedAt == null);
            if (existingMaterial != null)
            {
                existingMaterial.MaterialName = materialUpdate.MaterialName;
                existingMaterial.UpdatedByUserId = userId;
                existingMaterial.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }

        // Удаление материала курса
        public void DeleteElement(int materialId, int userId)
        {
            var material = _context.CourseMaterials.Find(materialId);
            if (material != null)
            {
                material.DeletedAt = DateTime.UtcNow;
                material.DeletedByUserId = userId; // Устанавливаем идентификатор пользователя, удалившего материал
                _context.SaveChanges();
            }
        }
    }
}