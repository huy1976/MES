namespace MesMicroservice.Application.Queries.Equipments;

public class OeeQuery: IRequest<IEnumerable<OeeViewModel>>
{
    public string EquipmentId { get; set; } = "";
    public DateTime From { get; set; } = DateTime.Now.AddDays(-1);
    public DateTime To { get; set; } = DateTime.Now;
    public double TimeFrameBySecond { get; set; } = 3600;
}
