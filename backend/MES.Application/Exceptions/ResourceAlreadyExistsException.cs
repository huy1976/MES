namespace MesMicroservice.Application.Exceptions;

[Serializable]
public class ResourceAlreadyExistsException : Exception
{
    public string ResourceType { get; } = "";
    public string ResourceId { get; } = "";

    public ResourceAlreadyExistsException() : base()
    {
    }

    public ResourceAlreadyExistsException(string? message) : base(message)
    {
    }

    public ResourceAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    public ResourceAlreadyExistsException(string resourceType, string resourceId)
        : base($"The entity of type '{resourceType}' with ID '{resourceId}' already exists.")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}
