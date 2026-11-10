using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Application.DTOs.MaterialDTOs
{
    public class MaterialUnitDTO
    {
        public string MaterialDefinitionId { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal ConversionValueToPrimaryUnit { get; set; }


    }
}
