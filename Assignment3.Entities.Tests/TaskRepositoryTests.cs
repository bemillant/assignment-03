namespace Assignment3.Entities.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Core;

public class TaskRepositoryTests : IDisposable
{
    private readonly KanbanContext context;
    private readonly TaskRepository taskRep;

    public TaskRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();


        //Tags
        var cleaning = new Tag("Cleaning") { id = 1 };
        var urgent = new Tag("Urgent") { id = 2 };
        var TBD = new Tag("TBD") { id = 3 };
        context.Tags.AddRange(cleaning, urgent, TBD);

        //Tasks
        var task1 = new Task("Clean Office") { id = 1, state = enumState.ACTIVE };
        var task2 = new Task("Do Taxes") { id = 2, state = enumState.NEW };
        var task3 = new Task("Go For A Run") { id = 3, state = enumState.RESOLVED };
        context.Tasks.AddRange(task1, task2, task3);

        var user1 = new User("Brian") { id = 1, email = "br@itu.dk" };
        context.Users.Add(user1);

        context.SaveChanges();

        this.context = context;
        taskRep = new TaskRepository(context);

    }


    //DO NOT KNOW HOW TO TEST THE THING WITH THE TIME
    //SEE 2.4 AND 2.6


    [Fact]
    public void Create_task_should_return_Created()
    {
        var list = new List<String> { "Cleaning", "Urgent" };
        var newTask = new TaskCreateDTO("NewTask", 1, "New Task that is something", list);

        var (response, id) = taskRep.Create(newTask);
        response.Should().Be(Response.Created);

        id.Should().Be(new TaskDTO(4, "NewTask", "Brian", list, State.New).Id);

    }


    [Fact]
    public void Delete_task_that_is_new_should_return_deleted()
    {
        var response = taskRep.Delete(2);
        response.Should().Be(Response.Deleted);
        context.Tasks.Find(2).Should().BeNull();
    }

    [Fact]
    public void Delete_task_that_is_active_should_return_state_removed()
    {
        var response = taskRep.Delete(1);
        context.Tasks.Find(1).state.Should().Be(enumState.REMOVED);
    }

    [Fact]
    public void Delete_task_that_is_Resolved_should_return_Conflict()
    {
        var response = taskRep.Delete(3);
        response.Should().Be(Response.Conflict);
        context.Tasks.Find(3).state.Should().Be(enumState.RESOLVED);
    }

    [Fact]
    public void Update_task_should_give_updated_tags()
    {

        var list = new List<string> { "Urgent", "TBD" };
        var urgent = new Tag("Urgent") { id = 2 };
        var TBD = new Tag("TBD") { id = 3 };
        var listT = new List<Tag> { urgent, TBD };


        var updateTask = new TaskUpdateDTO(1, "Clean Office", 1, null, list, State.Active);

        var resp = taskRep.Update(updateTask);
        resp.Should().Be(Response.Updated);

        //Should test this:
        //context.Tasks.Find(1).tags.Should().BeSameAs(listT);
        context.Tasks.Find(1).tags.Count.Should().Be(listT.Count);
    }

    [Fact]
    public void Assign_user_that_does_not_exist_return_BadRequest()
    {
        var updateTask = new TaskUpdateDTO(1, "Clean Office", 100, null, null, State.Active);

        var response = taskRep.Update(updateTask);
        response.Should().Be(Response.BadRequest);

    }

    public void Dispose()
    {
        context.Dispose();
    }
}
