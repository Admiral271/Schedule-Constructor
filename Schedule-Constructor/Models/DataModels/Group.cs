using System.ComponentModel.DataAnnotations;

namespace Schedule_Constructor.Models.DataModels
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}
