using MesMicroservice.Application.Queries.MaterialDefinitions;

namespace MesMicroservice.Application.Queries.MaterialLots;

public class MaterialLotViewModel
{
    public string MaterialLotId { get; set; }
    public string MaterialDefinition { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MaterialLotViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public MaterialLotViewModel(string materialLotId, string materialDefinition, decimal quantity, string unit)
    {
        MaterialLotId = materialLotId;
        MaterialDefinition = materialDefinition;
        Quantity = quantity;
        Unit = unit;
    }
}
