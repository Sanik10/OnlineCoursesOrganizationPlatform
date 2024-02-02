using System;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
        public int CreatedByUserId { get; set; }
    }
}