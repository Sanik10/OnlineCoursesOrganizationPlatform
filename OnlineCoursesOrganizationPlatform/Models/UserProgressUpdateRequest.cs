using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class UserProgressUpdateRequest
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [Required]
        public int Progress { get; set; }
        [Required]
        public bool Completed { get; set; }
    }
}