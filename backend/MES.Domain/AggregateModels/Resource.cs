namespace MesMicroservice.Domain.AggregateModels;
public class Resource: Entity, IAggregateRoot
{
    public string ResourceId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Resource() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Resource(string resourceId)
    {
        ResourceId = resourceId;
    }
}
