using System;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public interface IActionService
    {
        void LogAction(string actionType, string entityType, int entityId, int userId);
    }

    public class ActionService : IActionService
    {
        private readonly ApplicationDbContext _context;

        public ActionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Создание логов в журнал
        public void LogAction(string actionType, string entityType, int entityId, int userId)
        {
            var action = new Actions
            {
                ActionType = actionType,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Actions.Add(action);
            _context.SaveChanges();
        }
    }
}