using System;
using System.Collections.Generic;
using OnlineCoursesOrganizationPlatform.Models;

namespace OnlineCoursesOrganizationPlatform.Services
{
    public interface IService<T, TAddRequest, TUpdateRequest>
    {
        IEnumerable<T> GetAllElements();
        IEnumerable<T> GetAllActiveElements();
        IEnumerable<T> GetAllElementsByName(string elementName);
        IEnumerable<T> GetAllActiveElementsByName(string elementName);
        T GetElementById(int elementId);
        int AddElement(TAddRequest elementAddRequest, int userId);
        void UpdateElement(int elementId, TUpdateRequest updateElementRequest, int userId);
        void DeleteElement(int elementId, int userId);
    }
}