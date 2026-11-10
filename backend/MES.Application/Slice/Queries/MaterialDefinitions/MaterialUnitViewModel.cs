namespace MesMicroservice.Application.Queries.MaterialDefinitions;

public class MaterialUnitViewModel
{
    public string UnitId { get; set; }
    public string UnitName { get; set; }
    public decimal ConversionValueToPrimaryUnit { get; set; }

    public MaterialUnitViewModel(string unitId, string unitName, decimal conversionValueToPrimaryUnit)
    {
        UnitId = unitId;
        UnitName = unitName;
        ConversionValueToPrimaryUnit = conversionValueToPrimaryUnit;
    }
}
