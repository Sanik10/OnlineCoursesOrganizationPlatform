using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseDto
    {
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }
    }
}