using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class Tag
{
    public Tag(string name)
    {
        Name = name;
    }

    [Key] public int Id { get; set; }

    [Required] [StringLength(50)] public string Name { get; set; }

    public ICollection<Task> Tasks { get; set; }
}