namespace MesMicroservice.Application.Queries.Equipments;

public class OeeViewModel
{
    public string EquipmentId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal A { get; set; }
    public decimal P { get; set; }
    public decimal Q { get; set; }
    public decimal Oee { get; set; }

    public OeeViewModel(string equipmentId, DateTime startTime, DateTime endTime, decimal a, decimal p, decimal q, decimal oee)
    {
        EquipmentId = equipmentId;
        StartTime = startTime;
        EndTime = endTime;
        A = a;
        P = p;
        Q = q;
        Oee = oee;
    }
}
