using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseRating
    {
        [Key]
        public int RatingId { get; set; }
        public int CourseId { get; set; }
        public float Rating { get; set; }
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}