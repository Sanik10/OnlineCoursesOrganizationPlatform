using System;
using Microsoft.EntityFrameworkCore;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Action> Actions { get; set; }
        public DbSet<CourseRating> CourseRatings { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }

        // Методы для работы с данными

        // Пример метода для выборки пользователей по имени
        public User GetUserByName(string firstName, string lastName)
        {
            return Users.FirstOrDefault(u => u.FirstName == firstName && u.LastName == lastName);
        }

        // Пример метода для вставки нового пользователя
        public void AddUser(User user)
        {
            Users.Add(user);
            SaveChanges();
        }

        // Другие методы можно реализовать аналогично для других сущностей
    }
}