namespace MesMicroservice.Application.Commands.MaterialDefinitions.MaterialUnits;

[DataContract]
public class UpdateMaterialUnitViewModel
{
    [DataMember]
    public string UnitName { get; set; }
    [DataMember]
    public decimal ConversionValueToPrimaryUnit { get; set; }

    public UpdateMaterialUnitViewModel(string unitName, decimal conversionValueToPrimaryUnit)
    {
        UnitName = unitName;
        ConversionValueToPrimaryUnit = conversionValueToPrimaryUnit;
    }
}
