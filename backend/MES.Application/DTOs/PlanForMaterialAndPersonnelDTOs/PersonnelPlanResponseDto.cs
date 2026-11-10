namespace MesMicroservice.Application.DTOs.PlanForPersonnelAndPersonnelDTOs
{
    public class PersonnelPlanResponseDto
    {
        public DailyPersonnelPlanDto DailyPlan { get; set; } = new DailyPersonnelPlanDto();
        public WeeklyPersonnelPlanDto WeeklyPlan { get; set; } = new WeeklyPersonnelPlanDto();
        public MonthlyPersonnelPlanDto MonthlyPlan { get; set; } = new MonthlyPersonnelPlanDto();
    }

    public class MonthlyPersonnelPlanDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<WeeklyPersonnelPlanDto> Items { get; set; } = new List<WeeklyPersonnelPlanDto> { };
    }
    public class WeeklyPersonnelPlanDto
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public List<DailyPersonnelPlanDto> Items { get; set; } = new List<DailyPersonnelPlanDto> { };
    }

    public class DailyPersonnelPlanDto
    {
        public DateTime Date { get; set; }        // Ngày kế hoạch
        public List<PersonnelPlanItemDto> Items { get; set; } = new List<PersonnelPlanItemDto> { };
    }
    public class PersonnelPlanItemDto
    {
        public string WorkOrderId { get; set; } = string.Empty;
        public string PersonnelId { get; set; } = string.Empty;
        public string PersonnelName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Shift { get; set; } = string.Empty;
        public DateTime? PlanDate { get; set; }
    }
}

