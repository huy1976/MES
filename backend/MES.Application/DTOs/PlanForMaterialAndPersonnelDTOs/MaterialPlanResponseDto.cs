namespace MesMicroservice.Application.DTOs.PlanForMaterialAndPersonnelDTOs
{
    public class MaterialPlanResponseDto
    {
        public DailyMaterialPlanDto DailyPlan { get; set; } = new DailyMaterialPlanDto();
        public WeeklyMaterialPlanDto WeeklyPlan { get; set; } = new WeeklyMaterialPlanDto();
        public MonthlyMaterialPlanDto MonthlyPlan { get; set; } = new MonthlyMaterialPlanDto();
    }

    public class MonthlyMaterialPlanDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<WeeklyMaterialPlanDto> Items { get; set; } = new List<WeeklyMaterialPlanDto> { };
    }
    public class WeeklyMaterialPlanDto
    {
        public int Year { get; set; }
        public int Week { get; set; }         
        public List<DailyMaterialPlanDto> Items { get; set; } = new List<DailyMaterialPlanDto> { };
    }

    public class DailyMaterialPlanDto
    {
        public DateTime Date { get; set; }        // Ngày kế hoạch
        public List<MaterialPlanItemDto> Items { get; set; } = new List<MaterialPlanItemDto> {};
    }
    public class MaterialPlanItemDto
    {
        public string WorkOrderId { get; set; } = string.Empty;
        public string MaterialId { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime? PlanDate { get; set; }
    }

}
