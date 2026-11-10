global using MesMicroservice.Application.DTOs.PlanForPersonnelAndPersonnelDTOs;
global using MesMicroservice.Application.Queries.PlanForMaterialAndPersonnels;
using MesMicroservice.Application.DTOs.MaterialDTOs;

namespace MesMicroservice.Application.Queries.PlanForPersonnelAndPersonnels
{
    public class PlanForPersonnelQueryHandler : IRequestHandler<PlanForPersonnelQuery, PersonnelPlanResponseDto>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IPersonRepository _personRepository;

        public PlanForPersonnelQueryHandler(IWorkOrderRepository workOrderRepository, IPersonRepository personRepository)
        {
            _workOrderRepository = workOrderRepository;
            _personRepository = personRepository;
        }

        private List<WorkOrder> _cachedWos = new List<WorkOrder>();

        public async Task<PersonnelPlanResponseDto> Handle(PlanForPersonnelQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
            var persons = await _personRepository.GetAll() ?? new List<Person>();

            // Khởi tạo các biến kết quả
            DailyPersonnelPlanDto dailyPlan = new DailyPersonnelPlanDto();
            WeeklyPersonnelPlanDto weeklyPlan = new WeeklyPersonnelPlanDto();
            MonthlyPersonnelPlanDto monthlyPlan = new MonthlyPersonnelPlanDto();

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
                    return new PersonnelPlanResponseDto();
            }

            await LoadDataAsync(queryFromDate, queryToDate);



            switch (request.TimeRange)
            {
                case DashboardTimeRange.Day:
                    dailyPlan = new DailyPersonnelPlanDto
                    {
                        Date = today,
                        Items = GetPlanForSpecificDate(today, _cachedWos, persons)
                    };
                    break;

                case DashboardTimeRange.Week:
                    weeklyPlan = GenerateWeeklyPlanData(queryFromDate, _cachedWos, persons);
                    break;

                case DashboardTimeRange.Month:
                    monthlyPlan = GetMonthlyPersonnelPlanDto(today.Year, today.Month, queryFromDate, persons);
                    break;

                case DashboardTimeRange.MonthCustom:
                    var year = request.CustomYear ?? today.Year;
                    var month = request.CustomMonth ?? today.Month;

                    monthlyPlan = GetMonthlyPersonnelPlanDto(year, month, queryFromDate, persons);
                    break;
                default:
                    break;
            }


            return new PersonnelPlanResponseDto
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

        private List<PersonnelPlanItemDto> GetPlanForSpecificDate(
            DateTime targetDate,
            IEnumerable<WorkOrder> wos,
            IEnumerable<Person> persons)
        {
            var items = new List<PersonnelPlanItemDto>();
            var dayStart = targetDate.Date;

            foreach (var wo in wos)
            {
                if (!wo.StartTime.HasValue || !wo.EndTime.HasValue) continue;
                if (wo.EndTime < dayStart || wo.StartTime > dayStart.AddDays(1)) continue;

                var woPersonnels = wo.PersonWorkOrders ?? new List<PersonWorkOrder>();

                foreach (var pe in woPersonnels)
                {

                    var matId = pe.Person?.ResourceId;
                    var person = persons.FirstOrDefault(e => e.ResourceId == matId);

                    if (person is null) continue;

                    items.Add(new PersonnelPlanItemDto
                    {
                        WorkOrderId = wo.WorkOrderId,
                        PersonnelId = person.ResourceId,
                        PersonnelName = person.Name,
                        ClassName = pe.ClassName,
                        Shift = pe.ShiftName,
                        PlanDate = dayStart
                    });

                }
            }
            return items;
        }

        // --- CÁC HÀM TẠO CẤU TRÚC DỮ LIỆU (STRUCTURAL) ---

        private WeeklyPersonnelPlanDto GenerateWeeklyPlanData(
            DateTime weekStartDate,
            IEnumerable<WorkOrder> wos,
            IEnumerable<Person> persons)
        {
            var dailyPlans = new List<DailyPersonnelPlanDto>();
            for (int i = 0; i < 7; i++)
            {
                DateTime currentLoopDate = weekStartDate.AddDays(i);
                var itemsForDay = GetPlanForSpecificDate(currentLoopDate, wos, persons);

                dailyPlans.Add(new DailyPersonnelPlanDto
                {
                    Date = currentLoopDate,
                    Items = itemsForDay
                });
            }

            int weekNumber = GetIsoWeekOfYear(weekStartDate);
            return new WeeklyPersonnelPlanDto
            {
                Year = weekStartDate.Year,
                Week = weekNumber,
                Items = dailyPlans
            };
        }

        private MonthlyPersonnelPlanDto GetMonthlyPersonnelPlanDto(
            int year,
            int month,
            DateTime viewStartDate,
            IEnumerable<Person> persons)
        {
            var weeks = new List<WeeklyPersonnelPlanDto>();

            var lastDayOfMonth = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);

            var currentWeekStart = viewStartDate;


            while (currentWeekStart <= lastDayOfMonth)
            {
                var weeklyPlan = GenerateWeeklyPlanData(currentWeekStart, _cachedWos, persons);
                weeks.Add(weeklyPlan);
                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return new MonthlyPersonnelPlanDto
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
