namespace Assignment3.Entities;
using Microsoft.EntityFrameworkCore;

public class KanbanContext : DbContext
{

    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<User> Users => Set<User>();

    public KanbanContext(DbContextOptions<KanbanContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Making the enum to string
        modelBuilder
            .Entity<Task>()
            .Property(e => e.state)
            .HasConversion(
                s => s.ToString(),
                s => (enumState)Enum.Parse(typeof(enumState), s));

        //Making title required and setting length to be max 100
        modelBuilder.Entity<Task>()
            .Property(t => t.title)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.email)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.email)
            .IsUnique();

        modelBuilder.Entity<Tag>()
            .Property(t => t.name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.name)
            .IsUnique();





    }
}
