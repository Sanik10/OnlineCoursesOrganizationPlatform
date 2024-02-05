using System;
using System.Collections.Generic;
using System.Linq;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public class CourseService : IService<Course, CourseDto, CourseDto>
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Метод для получения всех курсов
        public IEnumerable<Course> GetAllElements()
        {
            return _context.Courses
                           .Where(c => c.CourseName != null && c.Description != null) // Проверка других полей
                           .ToList();
        }

        // Метод для получения курса по его идентификатору
        public Course GetElementById(int courseId)
        {
            return _context.Courses.Find(courseId);
        }

        // Метод для получения всех активных курсов
        public IEnumerable<Course> GetAllActiveElements()
        {
            return _context.Courses.Where(c => c.DeletedAt == null).ToList();
        }

        // Метод для получения всех курсов по имени
        public IEnumerable<Course> GetAllElementsByName(string elementName)
        {
            return _context.Courses.Where(c => c.CourseName.Contains(elementName)).ToList();
        }

        // Метод для получения всех активных курсов по имени
        public IEnumerable<Course> GetAllActiveElementsByName(string elementName)
        {
            return _context.Courses.Where(c => c.CourseName.Contains(elementName) && c.DeletedAt == null).ToList();
        }

        // Метод для добавления курса
        public int AddElement(CourseDto courseDto, int userId)
        {
            Course createCourse = new Course
            {
                CourseName = courseDto.CourseName,
                Description = courseDto.Description,
                CategoryId = courseDto.CategoryId,
                CreatedByUserId = userId,
                DeletedByUserId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _context.Courses.Add(createCourse);
            _context.SaveChanges();

            return createCourse.CourseId;
        }

        // Метод для изменения курса
        public void UpdateElement(int courseId, CourseDto courseDto, int userId)
        {
            var existingCourse = _context.Courses.FirstOrDefault(c => c.CourseId == courseId && c.DeletedAt == null);
            if (existingCourse != null)
            {
                existingCourse.CourseName = courseDto.CourseName;
                existingCourse.Description = courseDto.Description;
                existingCourse.CategoryId = courseDto.CategoryId;
                existingCourse.UpdatedAt = DateTime.UtcNow;
                existingCourse.UpdatedByUserId = userId;

                _context.SaveChanges();
            }
        }

        // Метод для удаления курса
        public void DeleteElement(int courseId, int userId)
        {
            var course = _context.Courses.Find(courseId);
            if (course != null)
            {
                course.DeletedAt = DateTime.UtcNow;
                course.DeletedByUserId = userId;

                _context.SaveChanges();
            }
        }
    }
}