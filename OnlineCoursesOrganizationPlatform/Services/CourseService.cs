using System;
using System.Collections.Generic;
using System.Linq;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public interface ICourseService
    {
        IEnumerable<Course> GetAllCourses();
        IEnumerable<Course> GetAllActiveCourses();
        Course GetCourseById(int courseId);
        void CreateCourse(Course course, int userId);
        void UpdateCourse(int courseId, CourseEditRequestDto courseDtom, int userId);
        void DeleteCourse(int courseId, int userId);
    }

    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return _context.Courses;
        }

        public IEnumerable<Course> GetAllActiveCourses()
        {
            return _context.Courses.Where(c => c.DeletedAt == null);
        }

        public Course GetCourseById(int courseId)
        {
            return _context.Courses.Find(courseId);
        }

        public void CreateCourse(Course course, int userId)
        {
            course.CreatedAt = DateTime.UtcNow;
            course.CreatedByUserId = userId;

            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public void UpdateCourse(int courseId, CourseEditRequestDto courseDto, int userId)
        {
            var course = _context.Courses.Find(courseId);
            if (course != null)
            {
                course.CourseName = courseDto.CourseName;
                course.Description = courseDto.Description;
                course.UpdatedAt = DateTime.UtcNow;
                // Установка пользователя, который внес последние изменения
                course.UpdatedByUserId = userId; // Предположим, что userId передается в качестве параметра метода

                _context.SaveChanges();
            }
        }

        public void DeleteCourse(int courseId, int userId)
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