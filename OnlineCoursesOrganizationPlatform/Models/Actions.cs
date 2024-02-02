using System;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class Action
    {
        public int ActionId { get; set; }
        public string ActionType { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}