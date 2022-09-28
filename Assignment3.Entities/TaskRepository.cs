namespace Assignment3.Entities;

public class TaskRepository : ITaskRepository
{
    KanbanContext context;

    public TaskRepository(KanbanContext context)
    {
        this.context = context;
    }

    public (Response Response, int TaskId) Create(TaskCreateDTO task)
    {
        var entity = context.Tasks.FirstOrDefault(c => c.title == task.Title);
        Response response;

        if (entity is null)
        {
            entity = new Task(task.Title) { state = enumState.NEW };

            context.Tasks.Add(entity);
            context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }
        return (response, entity.id);
    }

    public Response Delete(int taskId)
    {
        var entity = context.Tasks.FirstOrDefault(c => c.id == taskId);
        //Need to do this here? But only in this method?
        Response res = new Response();

        if (entity is not null)
        {
            if (entity.state.Equals(enumState.ACTIVE))
            {
                entity.state = enumState.REMOVED;
                context.SaveChanges();

                //Should set response to what? Delete?
                res = Response.Deleted;
            }
            else if (entity.state.Equals(enumState.RESOLVED) || entity.state.Equals(enumState.CLOSED) || entity.state.Equals(enumState.REMOVED))
            {
                res = Response.Conflict;
            }
            else if (entity.state.Equals(enumState.NEW))
            {
                context.Tasks.Remove(entity);
                context.SaveChanges();
                res = Response.Deleted;
            }
        }
        else
        {
            res = Response.NotFound;
        }
        return res;
    }

    public TaskDetailsDTO Read(int taskId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAll()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
    {
        throw new NotImplementedException();
    }

    public Response Update(TaskUpdateDTO task)
    {
        var entity = context.Tasks.Find(task.Id);
        Response response;

        if (entity is null)
        {
            response = Response.NotFound;
        }
        //if two tasks exists with the same titles but different ids
        else if (context.Tasks.FirstOrDefault(t => t.id != task.Id && t.title == task.Title) != null)
        {
            response = Response.Conflict;
        }
        else if (context.Users.Find(task.AssignedToId) is null)
        {
            response = Response.BadRequest;
        }
        else
        {
            entity.assignedTo = task.AssignedToId is not null ? context.Users.Find(task.AssignedToId) : entity.assignedTo;
            entity.description = task.Description is not null ? task.Description : entity.description;


            if (task.Tags is not null)
            {
                entity.tags = new List<Tag>();
                foreach (var tag in task.Tags!)
                {
                    foreach (var conTag in context.Tags)
                    {
                        if (conTag.name == tag)
                        {
                            entity.tags.Add(context.Tags.Find(conTag.id));
                        }
                    }
                }
                Console.WriteLine(entity.tags.First());
                Console.WriteLine(entity.tags.Last());
                Console.WriteLine(entity.tags);
                Console.WriteLine(entity.tags);
            }

            entity.state = (enumState)task.State;
            entity.title = task.Title;
            context.SaveChanges();
            response = Response.Updated;
        }

        return response;




    }
}
