using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class Tag
{
    [Key]
    public int id { get; set; }
    [Required]
    [StringLength(50)]
    public string name { get; set; }
    public ICollection<Task>? tasks { get; set; }
}
