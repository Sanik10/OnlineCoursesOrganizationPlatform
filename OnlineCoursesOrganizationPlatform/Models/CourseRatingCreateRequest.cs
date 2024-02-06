using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseRatingCreateRequest
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string RatingName { get; set; }

        [Required]
        public float Rating { get; set; }

        public string? Review { get; set; }
    }
}