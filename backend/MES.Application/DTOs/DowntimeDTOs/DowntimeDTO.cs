using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Application.DTOs.DowntimeDTOs;

public class DowntimeDTO
{
    public Equipment Equipment { get; set; }
    public int EquipmentId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get;  set; }
    public int DowntimeTypeId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public DowntimeDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public DowntimeDTO(Equipment equipment, int equipmentId, DateTime from, DateTime to, int downtimeTypeId)
    {
        Equipment = equipment;
        EquipmentId = equipmentId;
        From = from;
        To = to;
        DowntimeTypeId = downtimeTypeId;
    }
}
