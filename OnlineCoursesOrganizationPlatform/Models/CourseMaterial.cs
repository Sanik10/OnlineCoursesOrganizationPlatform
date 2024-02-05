using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseMaterial
    {
        [Key]
        public int MaterialId { get; set; }
        public int CourseId { get; set; }
        public int MaterialNumber { get; set; }
        public string MaterialName { get; set; }
        public string? MaterialContent { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
        public int CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
    }
}