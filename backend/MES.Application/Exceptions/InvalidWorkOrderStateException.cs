namespace MesMicroservice.Application.Exceptions;

[Serializable]
public class InvalidWorkOrderStateException: Exception
{
    public InvalidWorkOrderStateException()
    {
    }

    public InvalidWorkOrderStateException(string? message) : base(message)
    {
    }

    public InvalidWorkOrderStateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
