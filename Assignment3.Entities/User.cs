using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class User
{
    [Key]
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    public string? name { get; set; }
    [Required]
    [StringLength(100)]
    public string? email { get; set; }
    public ICollection<Task>? tasks { get; set; }

    public User(string name)
    {
        this.name = name;
    }


}
