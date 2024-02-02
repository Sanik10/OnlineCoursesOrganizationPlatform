using System;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseRating
    {
        public int RatingId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public float Rating { get; set; }
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}