using System.ComponentModel.DataAnnotations;

namespace Schedule_Constructor.Models.ViewModels
{
    public class AddGroupViewModel
    {
        [Required(ErrorMessage = "Вы не заполнили это поле")]
        [Display(Name = "Название")]
        public string Название { get; set; }

        [Required(ErrorMessage = "Вы не выбрали ни одной дисциплины")]
        public int[] Пары { get; set; }

        public int[] Hours { get; set; }
    }
}
