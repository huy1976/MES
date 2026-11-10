namespace MesMicroservice.Infrastructure.Exceptions;

[Serializable]
public class EntityNotFoundException : Exception
{
    public string EntityType { get; } = "";
    public string EntityId { get; } = "";

    public EntityNotFoundException() : base()
    {
    }

    public EntityNotFoundException(string? message) : base(message)
    {
    }

    public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    public EntityNotFoundException(string entityType, string entityId) : base($"The entity of type '{entityType}' with ID '{entityId}' cannot be found.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}
