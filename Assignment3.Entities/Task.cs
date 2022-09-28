using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public enum enumState
{
    NEW,
    ACTIVE,
    RESOLVED,
    CLOSED,
    REMOVED
}

public class Task
{
    [Key] public int id { get; set; }

    [Required] [StringLength(100)] public string? title { get; set; }

    public User? assignedTo { get; set; }

    [StringLength(int.MaxValue)] public string? description { get; set; }

    [Required] public enumState state { get; set; }

    public ICollection<Tag>? tags { get; set; }
}