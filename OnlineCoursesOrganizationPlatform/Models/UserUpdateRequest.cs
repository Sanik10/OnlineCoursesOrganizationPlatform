using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class UserUpdateRequest
    {
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }
        [Required]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }
    }
}