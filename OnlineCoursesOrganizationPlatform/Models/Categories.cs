using System;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
        public int CreatedByUserId { get; set; }
    }
}