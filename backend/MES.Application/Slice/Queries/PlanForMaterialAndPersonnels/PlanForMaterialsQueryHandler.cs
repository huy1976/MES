using MesMicroservice.Application.DTOs.PlanForMaterialAndPersonnelDTOs;
using MesMicroservice.Application.Queries.Dashboards;

namespace MesMicroservice.Application.Queries.PlanForMaterialAndPersonnels
{
    public class PlanForMaterialsQueryHandler : IRequestHandler<PlanForMaterialsQuery, MaterialPlanResponseDto>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public PlanForMaterialsQueryHandler(
            IWorkOrderRepository workOrderRepository,
            IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _workOrderRepository = workOrderRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        // Cache danh sách WO cho request này
        private List<WorkOrder> _cachedWos = new List<WorkOrder>();

        public async Task<MaterialPlanResponseDto> Handle(PlanForMaterialsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
            var materials = await _materialDefinitionRepository.GetAll() ?? new List<MaterialDefinition>();

            // Khởi tạo các biến kết quả
            DailyMaterialPlanDto dailyPlan = new DailyMaterialPlanDto();
            WeeklyMaterialPlanDto weeklyPlan = new WeeklyMaterialPlanDto();
            MonthlyMaterialPlanDto monthlyPlan = new MonthlyMaterialPlanDto();

            // 1. XÁC ĐỊNH PHẠM VI THỜI GIAN CẦN LẤY DỮ LIỆU (VIEW RANGE)
            DateTime queryFromDate;
            DateTime queryToDate;

            switch (request.TimeRange)
            {
                case DashboardTimeRange.Day:
                    queryFromDate = today;
                    queryToDate = today.AddDays(1).AddTicks(-1);
                    break;

                case DashboardTimeRange.Week:
                    queryFromDate = GetStartOfWeek(today);
                    queryToDate = queryFromDate.AddDays(7).AddTicks(-1);
                    break;

                case DashboardTimeRange.Month:
                    var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    queryFromDate = GetStartOfWeek(firstDayOfMonth);
                    var endOfLastWeek = GetStartOfWeek(lastDayOfMonth).AddDays(7).AddTicks(-1);
                    queryToDate = endOfLastWeek;
                    break;

                case DashboardTimeRange.MonthCustom:
                    var year = request.CustomYear ?? today.Year;
                    var month = request.CustomMonth ?? today.Month;

                    var customFirst = new DateTime(year, month, 1);
                    var customLast = customFirst.AddMonths(1).AddDays(-1);

                    queryFromDate = GetStartOfWeek(customFirst);
                    queryToDate = GetStartOfWeek(customLast).AddDays(7).AddTicks(-1);
                    break;

                default:
                    return new MaterialPlanResponseDto();
            }

            await LoadDataAsync(queryFromDate, queryToDate);

            switch (request.TimeRange)
            {
                case DashboardTimeRange.Day:
                    dailyPlan = new DailyMaterialPlanDto
                    {
                        Date = today,
                        Items = GetPlanForSpecificDate(today, _cachedWos, materials)
                    };
                    break;

                case DashboardTimeRange.Week:
                    weeklyPlan = GenerateWeeklyPlanData(queryFromDate, _cachedWos, materials);
                    break;

                case DashboardTimeRange.Month:
                    monthlyPlan = GetMonthlyMaterialPlanDto(today.Year, today.Month, queryFromDate, materials);
                    break;

                case DashboardTimeRange.MonthCustom:
                    var year = request.CustomYear ?? today.Year;
                    var month = request.CustomMonth ?? today.Month;
                    monthlyPlan = GetMonthlyMaterialPlanDto(year, month, queryFromDate, materials);
                    break;

                default:
                    break;
            }


            return new MaterialPlanResponseDto
            {
                DailyPlan = dailyPlan,
                WeeklyPlan = weeklyPlan,
                MonthlyPlan = monthlyPlan
            };
        }

        private async Task LoadDataAsync(DateTime from, DateTime to)
        {
            var rawWos = await _workOrderRepository.GetWOByTime(from, to) ?? new List<WorkOrder>();

            var statuses = new List<EStatus>
            {
                EStatus.ReadyForProduction,
                EStatus.InProduction,
                EStatus.Completed
            };

                _cachedWos = rawWos.Where(mo => statuses.Contains(mo.WorkOrderStatus)).ToList();
        }

        // --- LOGIC TÍNH TOÁN (CORE) ---

        private static double CalculateDailyMaterial(DateTime woStart, DateTime woEnd, double totalMaterial, DateTime day)
        {
            var dayStart = day.Date;
            var dayEnd = day.Date.AddDays(1).AddTicks(-1);

            var overlapStart = woStart > dayStart ? woStart : dayStart;
            var overlapEnd = woEnd < dayEnd ? woEnd : dayEnd;

            if (overlapEnd <= overlapStart) return 0;

            double woTotalSeconds = (woEnd - woStart).TotalSeconds;
            if (woTotalSeconds <= 0) return 0;

            double overlapSeconds = (overlapEnd - overlapStart).TotalSeconds;
            return totalMaterial * (overlapSeconds / woTotalSeconds);
        }

        private List<MaterialPlanItemDto> GetPlanForSpecificDate(
            DateTime targetDate,
            IEnumerable<WorkOrder> wos,
            IEnumerable<MaterialDefinition> materials)
        {
            var items = new List<MaterialPlanItemDto>();
            var dayStart = targetDate.Date;

            foreach (var wo in wos)
            {
                if (!wo.StartTime.HasValue || !wo.EndTime.HasValue) continue;
                if (wo.EndTime < dayStart || wo.StartTime > dayStart.AddDays(1)) continue;

                var woMaterials = wo.MaterialWorkOrders ?? new List<MaterialWorkOrder>();

                foreach (var ma in woMaterials)
                {

                    var matId = ma.MaterialDefinition?.ResourceId;
                    var materialDef = materials.FirstOrDefault(e => e.ResourceId == matId);

                    if (materialDef is null) continue;

                    double totalMaterial = (double)ma.Quantity.GetValue();
                    double dailyMaterial = CalculateDailyMaterial(
                        wo.StartTime.Value,
                        wo.EndTime.Value,
                        totalMaterial,
                        dayStart
                    );

                    var conversionUnit = materialDef.SecondaryUnits.FirstOrDefault();
                    dailyMaterial = conversionUnit is not null
                        ? dailyMaterial * (double)conversionUnit.ConversionValueToPrimaryUnit
                        : dailyMaterial;

                    if (dailyMaterial > 0)
                    {
                        var existingItem = items.FirstOrDefault(x =>
                            x.WorkOrderId == wo.WorkOrderId &&
                            x.MaterialId == materialDef.ResourceId);

                        if (existingItem != null)
                        {
                            existingItem.Quantity += dailyMaterial;
                        }
                        else
                        {
                            items.Add(new MaterialPlanItemDto
                            {
                                WorkOrderId = wo.WorkOrderId,
                                MaterialId = materialDef.ResourceId,
                                MaterialName = materialDef.Name,
                                Quantity = Math.Round(dailyMaterial, 3),
                                Unit = materialDef.PrimaryUnit,
                                PlanDate = dayStart
                            });
                        }
                    }
                }
            }
            return items;
        }

        // --- CÁC HÀM TẠO CẤU TRÚC DỮ LIỆU (STRUCTURAL) ---

        private WeeklyMaterialPlanDto GenerateWeeklyPlanData(
            DateTime weekStartDate,
            IEnumerable<WorkOrder> wos,
            IEnumerable<MaterialDefinition> materials)
        {
            var dailyPlans = new List<DailyMaterialPlanDto>();
            for (int i = 0; i < 7; i++)
            {
                DateTime currentLoopDate = weekStartDate.AddDays(i);
                var itemsForDay = GetPlanForSpecificDate(currentLoopDate, wos, materials);

                dailyPlans.Add(new DailyMaterialPlanDto
                {
                    Date = currentLoopDate,
                    Items = itemsForDay
                });
            }

            int weekNumber = GetIsoWeekOfYear(weekStartDate);
            return new WeeklyMaterialPlanDto
            {
                Year = weekStartDate.Year,
                Week = weekNumber,
                Items = dailyPlans
            };
        }

        private MonthlyMaterialPlanDto GetMonthlyMaterialPlanDto(
            int year,
            int month,
            DateTime viewStartDate,
            IEnumerable<MaterialDefinition> materials)
        {
            var weeks = new List<WeeklyMaterialPlanDto>();

            var lastDayOfMonth = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);

            var currentWeekStart = viewStartDate;


            while (currentWeekStart <= lastDayOfMonth)
            {
                var weeklyPlan = GenerateWeeklyPlanData(currentWeekStart, _cachedWos, materials);
                weeks.Add(weeklyPlan);
                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return new MonthlyMaterialPlanDto
            {
                Year = year,
                Month = month,
                Items = weeks
            };
        }

        // --- HELPERS ---

        private DateTime GetStartOfWeek(DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private int GetIsoWeekOfYear(DateTime time)
        {
            DayOfWeek day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                time,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }
    }
}