using MesMicroservice.Application.Exceptions;
using MesMicroservice.Application.Messages.ErrorDetails;
using MesMicroservice.Domain.Exceptions;
using MesMicroservice.Infrastructure.Exceptions;

namespace MesMicroservice.Application.Messages;

public class ErrorMessage
{
    public string ErrorCode { get; set; }
    public string Message { get; set; }
    public object Detail { get; set; }

    public ErrorMessage(string errorCode, string message, string detail)
    {
        ErrorCode = errorCode;
        Message = message;
        Detail = detail;
    }

    public ErrorMessage(Exception ex)
    {
        ErrorCode = "Unexpected";
        Message = ex.Message;
        var innerMessage = ex.InnerException?.Message;
        if (!string.IsNullOrEmpty(innerMessage))
        {
            Detail = innerMessage;
        } else
        {
            Detail = "";
        }
    }

    public ErrorMessage(ResourceNotFoundException ex)
    {
        ErrorCode = $"ResourceNotFound.{ex.ResourceType}";
        Message = $"The resource of type '{ex.ResourceType}' with ID '{ex.ResourceId}' cannot be found";
        Detail = new ResourceNotFoundErrorDetail(ex.ResourceType, ex.ResourceId);
    }

    public ErrorMessage(EntitiesNotFoundException ex)
    {
        ErrorCode = $"EntitiesNotFound.{ex.ResourceType}";
        Message = ex.Message;
        Detail = new EntitiesNotFoundErrorDetail(ex.ResourceType, ex.ResourceIds);
    }

    public ErrorMessage(WorkOrderNotScheduledException ex)
    {
        ErrorCode = "Domain.WorkOrderNotScheduled";
        Message = ex.Message;
        Detail = new WorkOrderNotScheduledErrorDetail(ex.WorkOrderId);
    }

    public ErrorMessage(EquipmentOccupiedException ex)
    {
        ErrorCode = "Domain.EquipmentOccupied";
        Message = ex.Message;
        Detail = new EquipmentOccupiedErrorDetail(ex.EquipmentId, ex.WorkOrderId);
    }

    public ErrorMessage(EntityDuplicationException ex)
    {
        ErrorCode = $"EntityDuplication.{ex.EntityType}";
        Message = $"The entity of type '{ex.EntityType}' with ID '{ex.EntityId}' already exists";
        Detail = new EntityDuplicationErrorDetail(ex.EntityType, ex.EntityId);
    }
}
