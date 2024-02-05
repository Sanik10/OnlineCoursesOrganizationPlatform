using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseDto
    {
        [Required]
        public string CourseName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}