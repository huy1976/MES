using MesMicroservice.Domain.AggregateModels.PersonAggregate;
using MesMicroservice.Domain.AggregateModels.PersonnelClassAggregate;
using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;
using MesMicroservice.Domain.AggregateModels.EquipmentClassAggregate;

namespace MesMicroservice.Algorithm.Services
{
    public class PersonnelSelectorService : IPersonnelSelectorService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IEquipmentClassRepository _equipmentClassRepository;

        // Cache work orders theo nhân sự
        private readonly Dictionary<string, List<WorkOrder>> _workOrdersByPerson = new();

        public PersonnelSelectorService(
            IPersonRepository personRepository,
            IWorkOrderRepository workOrderRepository,
            IEquipmentClassRepository equipmentClassRepository)
        {
            _personRepository = personRepository;
            _workOrderRepository = workOrderRepository;
            _equipmentClassRepository = equipmentClassRepository;
        }

        private static bool HasRuntimeConflict(
            Dictionary<string, List<(double Start, double End)>> reservations,
            Person person,
            Job job)
        {
            if (!reservations.TryGetValue(person.ResourceId, out var intervals))
                return false;

            return intervals.Any(x => job.StartTime < x.End && job.EndTime > x.Start);
        }

        private static void ReservePerson(
            Dictionary<string, List<(double Start, double End)>> reservations,
            Person person,
            Job job)
        {
            if (!reservations.ContainsKey(person.ResourceId))
                reservations[person.ResourceId] = new List<(double Start, double End)>();

            reservations[person.ResourceId].Add((job.StartTime, job.EndTime));
        }

        #region === Public API ===

        public async Task<Schedule> SelectPersonnel(Schedule schedule)
        {
            // PC02 hiện vẫn được giữ vì đây là nhóm nhân sự vận hành,
            // không phải hard-code số lượng công đoạn.
            var persons = await _personRepository.GetPersonByClassId("PC02");

            await BuildWorkOrderCache(persons);

            // Đọc loại năng lực cần thiết của từng EquipmentClass từ Database.
            // Ví dụ dữ liệu hiện tại:
            // EC01 -> Type = DRAW
            // EC02 -> Type = ENAMEL
            // EC03 -> Type = DRAW_ENAMEL
            //
            // Sau này nếu thêm:
            // EC04 -> Type = PACKAGING
            // thì chỉ cần nhân sự có property "packaging" = true.
            // Không cần sửa lại code ở đây.
            var capabilityKeyByEquipmentClass =
                await BuildCapabilityKeyByEquipmentClass();

            var allScheduledJobs = schedule.Jobs.Values
                .SelectMany(j => j)
                .Where(j => !string.IsNullOrWhiteSpace(j.AssignedMachineCode))
                .OrderBy(j => j.StartTime)
                .ToList();

            var machineByCode = schedule.Machines
                .Where(m => !string.IsNullOrWhiteSpace(m.Code))
                .GroupBy(m => m.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.First(),
                    StringComparer.OrdinalIgnoreCase);

            var runtimeReservations =
                new Dictionary<string, List<(double Start, double End)>>();

            foreach (var job in allScheduledJobs)
            {
                Machine? machine = null;

                if (machineByCode.TryGetValue(
                    job.AssignedMachineCode!,
                    out var scheduledMachine))
                {
                    machine = scheduledMachine;
                }
                else
                {
                    machine = job.EligibleMachines?
                        .FirstOrDefault(m => string.Equals(
                            m.Code,
                            job.AssignedMachineCode,
                            StringComparison.OrdinalIgnoreCase));
                }

                if (machine == null)
                    continue;

                // Không còn CapabilityMap:
                // EC01 -> draw
                // EC02 -> enamel
                // EC03 -> drawenamel
                //
                // Giờ đọc Type của EquipmentClass từ Database.
                if (!capabilityKeyByEquipmentClass.TryGetValue(
                        machine.ClassId,
                        out var capabilityKey) ||
                    string.IsNullOrWhiteSpace(capabilityKey))
                {
                    Console.WriteLine(
                        $"[Cảnh báo] EquipmentClass {machine.ClassId} chưa có property Type để xác định năng lực nhân sự.");
                    continue;
                }

                var capablePersons = FilterPersonsByCapability(
                    persons,
                    capabilityKey);

                if (!capablePersons.Any())
                {
                    Console.WriteLine(
                        $"[Cảnh báo] Không có nhân sự có năng lực '{capabilityKey}' để chạy máy {machine.Code} ({machine.ClassId})");
                    continue;
                }

                var p12 = capablePersons
                    .Where(p =>
                        HasShift(p, "shift1") &&
                        IsPersonAvailableForJob(p, job) &&
                        !HasRuntimeConflict(runtimeReservations, p, job))
                    .OrderBy(GetTotalWorkedHoursFromCache)
                    .FirstOrDefault();

                var p3 = capablePersons
                    .Where(p =>
                        HasShift(p, "shift3") &&
                        p.ResourceId != p12?.ResourceId &&
                        IsPersonAvailableForJob(p, job) &&
                        !HasRuntimeConflict(runtimeReservations, p, job))
                    .OrderBy(GetTotalWorkedHoursFromCache)
                    .FirstOrDefault();

                if (p12 == null || p3 == null)
                {
                    Console.WriteLine(
                        $"[Cảnh báo] Không đủ nhân sự không trùng lịch cho WO={job.Id}, Machine={machine.Code}, " +
                        $"Start={job.StartTime:F2}, End={job.EndTime:F2}"
                    );
                    continue;
                }

                job.Employees.Clear();
                job.Employees.Add(CreateEmployee(p12, "shift1"));
                job.Employees.Add(CreateEmployee(p3, "shift3"));

                ReservePerson(runtimeReservations, p12, job);
                ReservePerson(runtimeReservations, p3, job);
            }

            return schedule;
        }

        #endregion

        #region === Core selection logic ===

        private (Person? shift12, Person? shift3) SelectPersonnelPair(
            string machineCode,
            Job firstJob,
            List<Person> candidates)
        {
            var p12 = candidates
                .Where(p =>
                    HasShift(p, "shift1") &&
                    IsPersonAvailableForJob(p, firstJob))
                .OrderBy(GetTotalWorkedHoursFromCache)
                .FirstOrDefault();

            var p3 = candidates
                .Where(p =>
                    HasShift(p, "shift3") &&
                    p.ResourceId != p12?.ResourceId &&
                    IsPersonAvailableForJob(p, firstJob))
                .OrderBy(GetTotalWorkedHoursFromCache)
                .FirstOrDefault();

            return (p12, p3);
        }

        #endregion

        #region === Availability logic ===

        private bool IsPersonAvailableForJob(Person person, Job job)
        {
            if (!_workOrdersByPerson.TryGetValue(
                    person.ResourceId,
                    out var workOrders))
            {
                return true;
            }

            var now = DateTime.Now;

            var jobStart = now.AddHours(job.StartTime);
            var jobEnd = now.AddHours(job.EndTime);

            foreach (var wo in workOrders)
            {
                // Planned chưa được xem là lịch thực tế đang chiếm nhân sự.
                if (wo.WorkOrderStatus == EStatus.Planned)
                    continue;

                foreach (var eo in wo.EquipmentWorkOrders)
                {
                    var start =
                        eo.WorkOrder.ActuallyStartTime ??
                        eo.WorkOrder.StartTime;

                    var end =
                        eo.WorkOrder.ActuallyEndTime ??
                        eo.WorkOrder.EndTime;

                    if (start == null || end == null)
                        continue;

                    if (jobStart < end && jobEnd > start)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region === Cache & workload ===

        private async Task BuildWorkOrderCache(IEnumerable<Person> persons)
        {
            _workOrdersByPerson.Clear();

            foreach (var p in persons)
            {
                var wos = await _workOrderRepository
                    .GetByAssignedPersonId(p.ResourceId);

                _workOrdersByPerson[p.ResourceId] = wos.ToList();
            }
        }

        private double GetTotalWorkedHoursFromCache(Person person)
        {
            if (!_workOrdersByPerson.TryGetValue(
                    person.ResourceId,
                    out var workOrders))
            {
                return 0;
            }

            return workOrders
                .Where(wo => wo.WorkOrderStatus != EStatus.Planned)
                .SelectMany(wo => wo.EquipmentWorkOrders)
                .Sum(eo =>
                {
                    var start =
                        eo.WorkOrder.ActuallyStartTime ??
                        eo.WorkOrder.StartTime;

                    var end =
                        eo.WorkOrder.ActuallyEndTime ??
                        eo.WorkOrder.EndTime;

                    if (start == null || end == null)
                        return 0;

                    return (end.Value - start.Value).TotalHours;
                });
        }

        #endregion

        #region === Dynamic capability mapping ===

        private async Task<Dictionary<string, string>>
            BuildCapabilityKeyByEquipmentClass()
        {
            var equipmentClasses =
                await _equipmentClassRepository.GetEquipmentClasses();

            var result =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var equipmentClass in equipmentClasses)
            {
                var typeValue = equipmentClass.Properties?
                    .FirstOrDefault(p =>
                        p.Name.Equals(
                            "Type",
                            StringComparison.OrdinalIgnoreCase))
                    ?.Value?.ValueString;

                var capabilityKey =
                    NormalizeCapabilityKey(typeValue);

                if (string.IsNullOrWhiteSpace(capabilityKey))
                    continue;

                result[equipmentClass.ResourceId] = capabilityKey;
            }

            return result;
        }

        /// <summary>
        /// Chuẩn hóa tên Type của EquipmentClass thành tên capability của Person.
        ///
        /// Ví dụ:
        /// DRAW         -> draw
        /// ENAMEL       -> enamel
        /// DRAW_ENAMEL  -> drawenamel
        /// PACKAGING    -> packaging
        /// QUALITY_CHECK -> qualitycheck
        ///
        /// Nhờ vậy thêm loại máy/công đoạn mới không cần thêm if/else trong code.
        /// Chỉ cần dữ liệu EquipmentClass.Type và PersonnelCapability property
        /// dùng cùng một ý nghĩa.
        /// </summary>
        private static string NormalizeCapabilityKey(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return new string(
                value
                    .Trim()
                    .ToLowerInvariant()
                    .Where(char.IsLetterOrDigit)
                    .ToArray());
        }

        #endregion

        #region === Helpers ===

        private static Employee CreateEmployee(
            Person person,
            string shiftKey)
        {
            return new Employee
            {
                PersonId = person.ResourceId,
                Name = person.Name,
                shift1 = shiftKey == "shift1",
                shift2 = shiftKey == "shift1",
                shift3 = shiftKey == "shift3"
            };
        }

        private static bool HasShift(
            Person person,
            string shiftKey)
        {
            return person.PersonnelCapabilities?
                .Any(cap =>
                    cap.Properties?
                        .Any(p =>
                            p.Name.Equals(
                                shiftKey,
                                StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(
                                p.Value?.ValueString,
                                "true",
                                StringComparison.OrdinalIgnoreCase)
                        ) == true
                ) == true;
        }

        private static List<Person> FilterPersonsByCapability(
            List<Person> persons,
            string capabilityKey)
        {
            return persons
                .Where(p =>
                    p.PersonnelCapabilities?
                        .Any(cap =>
                            cap.Properties?
                                .Any(prop =>
                                    NormalizeCapabilityKey(prop.Name)
                                        == capabilityKey &&
                                    string.Equals(
                                        prop.Value?.ValueString,
                                        "true",
                                        StringComparison.OrdinalIgnoreCase)
                                ) == true
                        ) == true
                )
                .ToList();
        }

        #endregion
    }
}
