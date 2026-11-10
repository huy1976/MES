namespace MesMicroservice.Application.Queries.BillOfMaterials;

public class BillOfMaterialQuery : IRequest<BillOfMaterialViewModel?>
{
    public string BillOfMaterialId { get; set; }

    public BillOfMaterialQuery(string billOfMaterialId)
    {
        BillOfMaterialId = billOfMaterialId;
    }
}
