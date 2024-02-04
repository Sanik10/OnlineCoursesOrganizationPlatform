using System;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseMaterialRequest
    {
        public int CourseId { get; set; }
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public string FilePath { get; set; }
    }
}