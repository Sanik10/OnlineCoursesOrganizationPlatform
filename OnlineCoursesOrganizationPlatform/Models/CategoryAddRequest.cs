using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CategoryAddRequest
    {
        [Required]
        public string CategoryName { get; set; }
    }
}