namespace MesMicroservice.Application.Commands.MaterialDefinitions.MaterialUnits;

public class CreateMaterialUnitCommand : IRequest<bool>
{
    public string MaterialDefinitionId { get; set; }
    public string UnitId { get; set; }
    public string UnitName { get; set; }
    public decimal ConversionValueToPrimaryUnit { get; set; }

    public CreateMaterialUnitCommand(string materialDefinitionId, string unitId, string unitName, decimal conversionValueToPrimaryUnit)
    {
        MaterialDefinitionId = materialDefinitionId;
        UnitId = unitId;
        UnitName = unitName;
        ConversionValueToPrimaryUnit = conversionValueToPrimaryUnit;
    }
}
