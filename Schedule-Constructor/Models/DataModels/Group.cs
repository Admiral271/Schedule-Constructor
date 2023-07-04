using System.ComponentModel.DataAnnotations;

namespace Schedule_Constructor.Models.DataModels
{
    public class Group
    {
        [Key]
        public int Id_Group { get; set; }
        [Display(Name = "Название")]
        public string Name_Group { get; set; }
    }
}
