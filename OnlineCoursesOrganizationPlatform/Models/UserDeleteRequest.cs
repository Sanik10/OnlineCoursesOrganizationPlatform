using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineCoursesOrganizationPlatform.Models
{

    public class UserDeleteRequest
    {
        [Display(Name = "Введите пароль")]
        public string Password { get; set; }
        [Display(Name = "Подтвердите пароль")]
        public string ConfirmPassword { get; set; }
    }
}