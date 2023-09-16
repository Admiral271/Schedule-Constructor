using System;
using System.ComponentModel.DataAnnotations;

namespace Schedule_Constructor.Models.DataModels
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        public int GroupId { get; set; }

        [Display(Name = "Группа")]
        public Group Group { get; set; }

        [Display(Name = "День недели")]
        public int DayOfWeek { get; set; }

        [Display(Name = "Номер пары")]
        public int LessonNumber { get; set; }
        public int SubjectId { get; set; }

        [Display(Name = "Дисциплина")]
        public Subject Subject { get; set; }

        [Display(Name = "Четность недели")]
        public bool IsEvenWeek { get; set; }
    }
}
