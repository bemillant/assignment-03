using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public enum EnumState
{
    New,
    Active,
    Resolved,
    Closed,
    Removed
}

public class Task
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(100)] public string? Title { get; set; }

    public User? AssignedTo { get; set; }

    public string? Description { get; set; }

    [Required] public EnumState State { get; set; }

    public ICollection<Tag> Tags { get; set; }
}