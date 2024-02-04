using System;
using System.Collections.Generic;
using System.Linq;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public interface ICourseMaterialService
    {
        IEnumerable<CourseMaterial> GetAllMaterialsByCourseId(int courseId);
        CourseMaterial GetMaterialById(int materialId);
        void AddMaterial(CourseMaterial material, int userId);
        void UpdateMaterial(CourseMaterial material, int userId);
        void DeleteMaterial(int materialId, int userId);
    }

    public class CourseMaterialService : ICourseMaterialService
    {
        private readonly ApplicationDbContext _context;

        public CourseMaterialService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CourseMaterial> GetAllMaterialsByCourseId(int courseId)
        {
            return _context.CourseMaterials.Where(m => m.CourseId == courseId && m.DeletedAt == null).ToList();
        }

        public CourseMaterial GetMaterialById(int materialId)
        {
            return _context.CourseMaterials.FirstOrDefault(m => m.MaterialId == materialId && m.DeletedAt == null);
        }

        public void AddMaterial(CourseMaterial material, int userId)
        {
            // Устанавливаем идентификатор пользователя, создавшего материал курса
            material.CreatedByUserId = userId;

            // Устанавливаем даты создания
            material.CreatedAt = DateTime.UtcNow;
            material.UpdatedAt = null;

            // Добавляем материал курса
            _context.CourseMaterials.Add(material);
            _context.SaveChanges();
        }

        public void UpdateMaterial(CourseMaterial material, int userId)
        {
            // Устанавливаем идентификатор пользователя, обновившего материал курса
            material.UpdatedByUserId = userId;

            // Устанавливаем дату обновления
            material.UpdatedAt = DateTime.UtcNow;

            // Обновляем материал курса
            _context.CourseMaterials.Update(material);
            _context.SaveChanges();
        }

        public void DeleteMaterial(int materialId, int userId)
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