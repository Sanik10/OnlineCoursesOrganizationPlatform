﻿using System;
namespace OnlineCoursesOrganizationPlatform.Models
{
	public class UserRegistrationRequest
	{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? CreatedByUserId { get; set; }
    }
}