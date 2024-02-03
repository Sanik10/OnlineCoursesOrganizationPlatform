using System;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesOrganizationPlatform.Controllers;
using OnlineCoursesOrganizationPlatform.Models;
using OnlineCoursesOrganizationPlatform.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly string _secretKey;

        public JwtService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public string GenerateToken(User user)
        {
            // Создаем утверждения (claims) для токена
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // Другие утверждения по желанию
            };

            // Создаем ключ
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));


            // Создаем учетные данные для подписи
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Создаем JWT токен
            var token = new JwtSecurityToken(
                issuer: "TrophyRaceTop",
                audience: "OnlineCoursesPlatform",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Время жизни токена
                signingCredentials: creds
            );

            // Кодируем токен в строку
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
    }
}