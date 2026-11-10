namespace MesMicroservice.Application.Queries.SegmentModels;

public class EquipmentSegmentSpecificationsByProcessSegmentIdQuery : IRequest<IEnumrable<EquipmentSegmentSpecificationViewModel>>
{
   public string ProcessSegmentId { get; set; }
   public EquipmentSegmentSpecificationsByProcessSegmentIdQuery(string processSegmentId)
    {
        ProcessSegmentId = processSegmentId;
    }
}
