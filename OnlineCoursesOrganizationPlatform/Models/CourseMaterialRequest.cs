﻿using System.ComponentModel.DataAnnotations;

namespace OnlineCoursesOrganizationPlatform.Models
{
    public class CourseMaterialRequest
    {
        [Display(Name = "Идентификатор курса")]
        public int CourseId { get; set; }

        [Display(Name = "Номер материала в курсе")]
        public int MaterialNumber { get; set; }

        [Display(Name = "Название материала")]
        public string MaterialName { get; set; }

        [Display(Name = "Содержание материала")]
        public string MaterialContent { get; set; }

        [Display(Name = "Ссылка на допю файл")]
        public string FilePath { get; set; }
    }
}