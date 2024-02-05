using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Электронная почта")]
        [EmailAddress(ErrorMessage = "Неверный формат адреса электронной почты")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Дата обновления")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Дата удаления")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeletedAt { get; set; }

        [Display(Name = "Удален пользователем")]
        public int? DeletedByUserId { get; set; }

        [Display(Name = "Приглашен пользователем")]
        public int? CreatedByUserId { get; set; }
    }
}