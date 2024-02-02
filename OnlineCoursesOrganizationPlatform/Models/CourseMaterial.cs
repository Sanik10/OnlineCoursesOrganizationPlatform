using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseMaterial
    {
        [Key]
        public int MaterialId { get; set; }
        public int CourseId { get; set; }
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}