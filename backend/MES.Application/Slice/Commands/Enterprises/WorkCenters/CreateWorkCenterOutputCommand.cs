using MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;

namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class CreateWorkCenterOutputCommand : IRequest<bool>
{
    public string AbsolutePath { get; set; }
    public string MaterialDefinitionId { get; set; }
    public decimal Output { get; set; }
    public string MaterialUnitId { get; set; }

    public CreateWorkCenterOutputCommand(string absolutePath, string materialDefinitionId, decimal output, string materialUnitId)
    {
        AbsolutePath = absolutePath;
        MaterialDefinitionId = materialDefinitionId;
        Output = output;
        MaterialUnitId = materialUnitId;
    }
}
