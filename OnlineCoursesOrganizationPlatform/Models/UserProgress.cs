using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class UserProgress
    {
        [Key]
        public int ProgressId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int MaterialId { get; set; }
        public int Progress { get; set; }
        public DateTime LastAccessed { get; set; }
        public bool Completed { get; set; }
    }
}