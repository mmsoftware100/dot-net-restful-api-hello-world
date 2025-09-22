using System.ComponentModel.DataAnnotations;

namespace TestAPI.Models
{
    public class Blood
    {
        [Key]
        public int Id { get; set; }
        public string Name_English { get; set; }
        public string Name_Myanmar { get; set; }
    }
}
