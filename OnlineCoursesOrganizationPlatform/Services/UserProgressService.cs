using System;
using System.Collections.Generic;
using System.Linq;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public class UserProgressService
    {
        private readonly ApplicationDbContext _context;

        public UserProgressService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Обновление/Добавление прогресса
        public void UpdateProgress(UserProgressUpdateRequest progressRequest, int userId)
        {
            UserProgress userProgress = _context.UserProgress.FirstOrDefault(up => up.UserId == userId && up.MaterialId == progressRequest.MaterialId);

            if (userProgress == null)
            {
                userProgress = new UserProgress
                {
                    UserId = userId,
                    CourseId = progressRequest.CourseId,
                    MaterialId = progressRequest.MaterialId,
                    Progress = progressRequest.Progress,
                    LastAccessed = DateTime.UtcNow,
                    Completed = progressRequest.Completed
                };
                _context.UserProgress.Add(userProgress);
            }
            else
            {
                userProgress.Progress = progressRequest.Progress;
                userProgress.LastAccessed = DateTime.UtcNow;
                userProgress.Completed = progressRequest.Completed;
            }

            _context.SaveChanges();
        }

        // Получение прогресса юзера
        public IEnumerable<UserProgress> GetProgressByUserId(int userId)
        {
            return _context.UserProgress.Where(up => up.UserId == userId).ToList();
        }
    }
}