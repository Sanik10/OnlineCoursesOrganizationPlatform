namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseEditRequestDto
    {
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
    }
}