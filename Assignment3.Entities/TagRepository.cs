namespace Assignment3.Entities;

public class TagRepository : ITagRepository
{
    private readonly KanbanContext context;

    public TagRepository(KanbanContext context)
    {
        this.context = context;
    }

    public (Response Response, int TagId) Create(TagCreateDTO tag)
    {
        var entity = context.Tags.FirstOrDefault(c => c.name == tag.Name);
        Response response;

        if (entity is null)
        {
            entity = new Tag(tag.Name);

            context.Tags.Add(entity);
            context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.id);
    }

    public Response Delete(int tagId, bool force = false)
    {
        var entity = context.Tags.FirstOrDefault(c => c.id == tagId);
        Response response;

        if (entity is not null)
        {
            if (entity.tasks is not null)
            {
                if (!force)
                {
                    response = Response.Conflict;
                }
                else
                {
                    context.Tags.Remove(entity);
                    context.SaveChanges();
                    response = Response.Deleted;
                }
            }
            else
            {
                context.Tags.Remove(entity);
                context.SaveChanges();
                response = Response.Deleted;
            }
        }
        else
        {
            response = Response.NotFound;
        }

        return response;
    }

    public TagDTO Read(int tagId)
    {
        var tags = from t in context.Tags
            where t.id == tagId
            select new TagDTO(t.id, t.name);

        return tags.FirstOrDefault();
    }

    public IReadOnlyCollection<TagDTO> ReadAll()
    {
        var tags = from t in context.Tags
            select new TagDTO(t.id, t.name);

        return tags.ToList();
    }

    public Response Update(TagUpdateDTO tag)
    {
        var entity = context.Tags.Find(tag.Id);
        Response response;

        if (entity is null)
        {
            response = Response.NotFound;
        }
        //if two tags exists with the same name but different ids
        else if (context.Tags.FirstOrDefault(t => t.id != tag.Id && t.name == tag.Name) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            entity.name = tag.Name;
            context.SaveChanges();
            response = Response.Updated;
        }

        return response;
    }
}