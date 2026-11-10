namespace MesMicroservice.Application.Exceptions;

[Serializable]
public class ResourceNotFoundException : Exception
{
    public string ResourceType { get; } = "";
    public string ResourceId { get; } = "";

    public ResourceNotFoundException() : base()
    {
    }

    public ResourceNotFoundException(string? message) : base(message)
    {
    }

    public ResourceNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    public ResourceNotFoundException(string resourceType, string resourceId) : base($"The entity of type '{resourceType}' with ID '{resourceId}' cannot be found.")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}
