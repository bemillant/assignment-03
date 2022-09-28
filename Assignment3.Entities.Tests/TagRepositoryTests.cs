using Assignment3.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Entities.Tests;

public class TagRepositoryTests : IDisposable
{
    private readonly KanbanContext context;
    private readonly TagRepository tagRep;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();

        context.Tags.AddRange(new Tag("Cleaning") {id = 1}, new Tag("Urgent") {id = 2}, new Tag("TBD") {id = 3});
        context.SaveChanges();

        this.context = context;
        tagRep = new TagRepository(context);
    }


    public void Dispose()
    {
        context.Dispose();
    }


    [Fact]
    public void Create_Tag_Should_Give_Tag()
    {
        var (response, id) = tagRep.Create(new TagCreateDTO("HighPrio"));

        response.Should().Be(Response.Created);

        id.Should().Be(new TagDTO(4, "HighPrio").Id);
    }

    [Fact]
    public void Create_Tag_Should_give_conflict_since_Tag_exists()
    {
        var (response, id) = tagRep.Create(new TagCreateDTO("Cleaning"));

        response.Should().Be(Response.Conflict);

        id.Should().Be(new TagDTO(1, "Cleaning").Id);
    }

    [Fact]
    public void Delete_existing_tag_not_in_use()
    {
        var response = tagRep.Delete(1);
        response.Should().Be(Response.Deleted);
        context.Tags.Find(1).Should().BeNull();
    }

    [Fact]
    public void Delete_non_existing_tag_return_notFound()
    {
        var response = tagRep.Delete(100);
        response.Should().Be(Response.NotFound);
        context.Tags.Find(100).Should().BeNull();
    }

    [Fact]
    public void Delete_tag_in_use_without_using_force_should_give_conflict()
    {
        var task1 = new Task {title = "Clean Office", id = 1, state = (enumState) State.Active};
        var task2 = new Task {title = "Do Taxes", id = 2, state = (enumState) State.New};
        var list = new List<Task> {task1, task2};
        context.Tags.Find(1)!.tasks = list;

        var response = tagRep.Delete(1);
        response.Should().Be(Response.Conflict);
        context.Tags.Find(1).Should().NotBeNull();
    }


    [Fact]
    public void Read_return_the_right_tag()
    {
        var tagD = new TagDTO(1, "Cleaning");
        var result = tagRep.Read(1);
        result.Should().Be(tagD);
    }

    [Fact]
    public void ReadAll_Should_return_all_the_tags()
    {
        var t1 = new TagDTO(1, "Cleaning");
        var t2 = new TagDTO(2, "Urgent");
        var t3 = new TagDTO(3, "TBD");
        var listOfTags = new List<TagDTO> {t1, t2, t3};
        var result = tagRep.ReadAll();

        result.Should().BeEquivalentTo(listOfTags);
    }


    [Fact]
    public void Update_tag_should_give_updated()
    {
        var response = tagRep.Update(new TagUpdateDTO(1, "Office work"));
        response.Should().Be(Response.Updated);

        var entity = context.Tags.Find(1)!;
        entity.name.Should().Be("Office work");
    }
}