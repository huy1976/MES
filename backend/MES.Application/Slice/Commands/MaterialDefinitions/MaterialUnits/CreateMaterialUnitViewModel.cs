namespace MesMicroservice.Application.Commands.MaterialDefinitions.MaterialUnits;

[DataContract]
public class CreateMaterialUnitViewModel
{
    [DataMember]
    public string UnitId { get; set; }
    [DataMember]
    public string UnitName { get; set; }
    [DataMember]
    public decimal ConversionValueToPrimaryUnit { get; set; }

    public CreateMaterialUnitViewModel(string unitId, string unitName, decimal conversionValueToPrimaryUnit)
    {
        UnitId = unitId;
        UnitName = unitName;
        ConversionValueToPrimaryUnit = conversionValueToPrimaryUnit;
    }
}
