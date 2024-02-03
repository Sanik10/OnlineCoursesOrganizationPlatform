using System;
namespace OnlineCoursesOrganizationPlatform.Services
{
    public interface ITokenService
    {
        string Token { get; set; }
    }

    public class TokenService : ITokenService
    {
        private string _token;

        public string Token
        {
            get => _token;
            set => _token = value;
        }
    }
}

