using System.ComponentModel.DataAnnotations;

namespace Schedule_Constructor.Models.DataModels
{
    public class Subject
    {
        [Key]
        public int Id
        { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Кабинет")]
        public string Cabinet { get; set; }
        [Display(Name = "Преподаватель")]
        public string Teacher { get; set; }
    }
}
