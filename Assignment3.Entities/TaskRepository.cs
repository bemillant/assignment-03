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
        var entity = context.Tasks.FirstOrDefault(c => c.Title == task.Title);
        Response response;

        if (entity is null)
        {
            entity = new Task { Title = task.Title, State = State.New };

            context.Tasks.Add(entity);
            context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }
        return (response, entity.Id);
    }

    public Response Delete(int taskId)
    {
        var entity = context.Tasks.FirstOrDefault(c => c.Id == taskId);
        //Need to do this here? But only in this method?
        Response res = new Response();

        if (entity is not null)
        {
            if (entity.State.Equals(State.Active))
            {
                entity.State = State.Removed;
                context.SaveChanges();

                //Should set response to what? Delete?
                res = Response.Deleted;
            }
            else if (entity.State.Equals(State.Resolved) || entity.State.Equals(State.Closed) || entity.State.Equals(State.Removed))
            {
                res = Response.Conflict;
            }
            else if (entity.State.Equals(State.New))
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

    public IReadOnlyCollection<TaskDTO> ReadAllByState(State State)
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
        else if (context.Tasks.FirstOrDefault(t => t.Id != task.Id && t.Title == task.Title) != null)
        {
            response = Response.Conflict;
        }
        else if (context.Users.Find(task.AssignedToId) is null)
        {
            response = Response.BadRequest;
        }
        else
        {
            entity.AssignedTo = task.AssignedToId is not null ? context.Users.Find(task.AssignedToId) : entity.AssignedTo;
            entity.Description = task.Description is not null ? task.Description : entity.Description;


            if (task.Tags is not null)
            {
                entity.Tags = new List<Tag>();
                foreach (var tag in task.Tags!)
                {
                    foreach (var conTag in context.Tags)
                    {
                        if (conTag.Name == tag)
                        {
                            entity.Tags.Add(context.Tags.Find(conTag.Id));
                        }
                    }
                }
            }

            entity.State = task.State;
            entity.Title = task.Title;
            context.SaveChanges();
            response = Response.Updated;
        }

        return response;




    }
}
