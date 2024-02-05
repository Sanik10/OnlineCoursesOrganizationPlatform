using System;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public class CourseRatingsService : IService<CourseRating, CourseRatingCreateRequest, CourseRatingUpdateRequest>
    {
        private readonly ApplicationDbContext _context;

        public CourseRatingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получение всех оценок
        public IEnumerable<CourseRating> GetAllElements()
        {
            return _context.CourseRatings.ToList();
        }

        // Получение всех активных оценок
        public IEnumerable<CourseRating> GetAllActiveElements()
        {
            return _context.CourseRatings.Where(r => r.DeletedAt == null).ToList();
        }

        // Получение всех оценок по имени
        public IEnumerable<CourseRating> GetAllElementsByName(string elementName)
        {
            return _context.CourseRatings.Where(r => r.RatingName.Contains(elementName)).ToList();
        }

        // Получение всех активных оценок по имени
        public IEnumerable<CourseRating> GetAllActiveElementsByName(string elementName)
        {
            return _context.CourseRatings.Where(r => r.RatingName.Contains(elementName) && r.DeletedAt == null).ToList();
        }

        // Получение всех оценоки по айди
        public CourseRating GetElementById(int courseRatingId)
        {
            return _context.CourseRatings.Find(courseRatingId);
        }

        // Добавление оценки
        public int AddElement(CourseRatingCreateRequest courseRatingRequest, int userId)
        {
            CourseRating newCourseRating = new CourseRating
            {
                CourseId = courseRatingRequest.CourseId,
                Rating = courseRatingRequest.Rating,
                RatingName = courseRatingRequest.RatingName,
                Review = courseRatingRequest.Review,
                CreatedByUserId = userId,
                DeletedByUserId = null,
                UpdatedByUserId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                DeletedAt = null,
            };

            // Добавляем материал курса
            _context.CourseRatings.Add(newCourseRating);
            _context.SaveChanges();

            return newCourseRating.RatingId;
        }

        // Редактирование оценки
        public void UpdateElement(int courseRatingId, CourseRatingUpdateRequest courseRatingRequest, int userId)
        {
            var ratingToUpdate = _context.CourseRatings.Find(courseRatingId);
            if (ratingToUpdate != null)
            {
                ratingToUpdate.RatingName = courseRatingRequest.RatingName;
                ratingToUpdate.Rating = courseRatingRequest.Rating;
                ratingToUpdate.Review = courseRatingRequest.Review;
                ratingToUpdate.UpdatedAt = DateTime.UtcNow;
                ratingToUpdate.UpdatedByUserId = userId;
                _context.SaveChanges();
            }
        }

        // Удаление оценки
        public void DeleteElement(int courseRatingId, int userId)
        {
            var ratingToDelete = _context.CourseRatings.Find(courseRatingId);
            if (ratingToDelete != null)
            {
                ratingToDelete.DeletedAt = DateTime.UtcNow;
                ratingToDelete.DeletedByUserId = userId;
                _context.SaveChanges();
            }
        }

        // Проверка
        public bool CheckIfRatingExists(int ratingId)
        {
            return _context.CourseRatings.Any(r => r.RatingId == ratingId);
        }
    }
}

