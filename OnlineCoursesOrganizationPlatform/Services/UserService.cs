using OnlineCoursesOrganizationPlatform.Models;
using System;
using System.Linq;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        User GetById(int id);
        void Logout();
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User Authenticate(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == email && x.Password == password);

            if (user == null)
                return null;

            // Аутентификация успешна
            return user;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public void Logout()
        {
            // Реализуйте здесь логику выхода из системы, например, очистку текущего токена доступа или отзыв токена.
            // Этот метод может быть пустым, если в вашем приложении не требуется специальных действий при выходе пользователя.
        }
    }
}