using System;
using System.Collections.Generic;
using System.Linq;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public class CategoryService : IService<Category, CategoryAddRequest, CategoryAddRequest>
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Метод для получения всех категорий
        public IEnumerable<Category> GetAllElements()
        {
            return _context.Categories.ToList();
        }

        // Метод для получения всех категорий по айди
        public Category GetElementById(int categoryId)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId && c.DeletedAt == null);
        }

        // Метод для получения всех активных категорий
        public IEnumerable<Category> GetAllActiveElements()
        {
            return _context.Categories.Where(c => c.DeletedAt == null).ToList();
        }

        // Метод для получения всех категорий по имени
        public IEnumerable<Category> GetAllElementsByName(string elementName)
        {
            return _context.Categories.Where(c => c.CategoryName.Contains(elementName)).ToList();
        }

        // Метод для получения всех активных категорий по имени
        public IEnumerable<Category> GetAllActiveElementsByName(string elementName)
        {
            return _context.Categories.Where(c => c.CategoryName.Contains(elementName) && c.DeletedAt == null).ToList();
        }

        // Метод для добавления категории
        public int AddElement(CategoryAddRequest categoryRequest, int userId)
        {
            Category newCategory = new Category
            {
                CategoryName = categoryRequest.CategoryName,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = null,
                UpdatedAt = null,
                CreatedByUserId = userId,
                DeletedByUserId = null,
                UpdatedByUserId = null
            };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return newCategory.CategoryId;
        }

        // Метод для изменения категории
        public void UpdateElement(int categoryId, CategoryAddRequest categoryAddRequest, int userId)
        {
            Category existingCategory = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId && c.DeletedAt == null);
            if (existingCategory != null)
            {
                existingCategory.CategoryName = categoryAddRequest.CategoryName;
                existingCategory.UpdatedAt = DateTime.UtcNow;
                existingCategory.UpdatedByUserId = userId;
                _context.SaveChanges();
            }
        }

        // Метод для удаления категории
        public void DeleteElement(int categoryId, int userId)
        {
            Category category = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId && c.DeletedAt == null);
            if (category != null)
            {
                category.DeletedAt = DateTime.UtcNow;
                category.DeletedByUserId = userId;
                _context.SaveChanges();
            }
        }
    }
}