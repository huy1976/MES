namespace MesMicroservice.Application.Commands.MaterialDefinitions.MaterialUnits;

public class DeleteMaterialUnitCommand : IRequest<bool>
{
    public string MaterialDefinitionId { get; set; }
    public string UnitId { get; set; }

    public DeleteMaterialUnitCommand(string materialDefinitionId, string unitId)
    {
        MaterialDefinitionId = materialDefinitionId;
        UnitId = unitId;
    }
}
