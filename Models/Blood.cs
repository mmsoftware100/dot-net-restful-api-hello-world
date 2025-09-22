using System.ComponentModel.DataAnnotations;

namespace TestAPI.Models
{
    public class Blood
    {
        [Key]
        public int Id { get; set; }
        public required string Name_English { get; set; }
        public required string Name_Myanmar { get; set; }
    }
}
