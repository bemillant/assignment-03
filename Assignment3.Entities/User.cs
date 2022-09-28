using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class User
{
    [Key] public int Id { get; set; }

    [Required][StringLength(100)] public string? Name { get; set; }

    public User(string name)
    {
        this.Name = name;
    }


    [Required][StringLength(100)] public string? Email { get; set; }

    public ICollection<Task> Tasks { get; set; }
}
