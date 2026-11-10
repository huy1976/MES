using Azure.Core;

namespace MesMicroservice.Application.Queries.Dashboards
{
    public class DashboardQueryHandler : IRequestHandler<DashboardQuery, DashboardDTO>
    {
        private  readonly IManufacturingOrderRepository _moRepository;
        private readonly IOperationsDefinitionRepository _operationsDefinitionRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        private readonly IWorkOrderProgressRecord _workOrderProgressRecordRepository;
        private readonly IWorkOrderErrorRecord _workOrderErrorRecordRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IWorkOrderRepository _workOrderRepository;

        public DashboardQueryHandler(IManufacturingOrderRepository moRepository, 
                                     IOperationsDefinitionRepository operationsDefinitionRepository, 
                                     IMaterialDefinitionRepository materialDefinitionRepository, 
                                     IWorkOrderProgressRecord workOrderProgressRecord,
                                     IEquipmentRepository equipmentRepository,
                                     IWorkOrderRepository workOrderRepository,
                                     IWorkOrderErrorRecord workOrderErrorRecord)
        {
            _moRepository = moRepository;
            _operationsDefinitionRepository = operationsDefinitionRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
            _workOrderProgressRecordRepository = workOrderProgressRecord;
            _equipmentRepository = equipmentRepository;
            _workOrderRepository = workOrderRepository;
            _workOrderErrorRecordRepository = workOrderErrorRecord;
        }

        private List<ManufacturingOrder> MOs = new List<ManufacturingOrder>();
        private List<ManufacturingOrder> mos = new List<ManufacturingOrder>();

        public async Task<DashboardDTO> Handle(DashboardQuery request, CancellationToken cancellationToken)
        {
            // Khai báo
            var dashboardData = new DashboardDTO();
            var timeRange = new TimeRange();

            var now = DateTime.Now;          // Local Time
            var today = DateTime.Today;      // 00:00 hôm nay

            switch (request.TimeRange)
            {
                case DashboardTimeRange.Day:
                    timeRange.From = today;
                    timeRange.To = today.AddDays(1).AddTicks(-1);
                    break;

                case DashboardTimeRange.Week:
                    // ISO-8601: tuần bắt đầu từ Monday
                    int delta = DayOfWeek.Monday - today.DayOfWeek;
                    if (delta > 0) delta -= 7;  // nếu hôm nay là Chủ Nhật → trừ thêm 7 ngày

                    timeRange.From = today.AddDays(delta);                 // Monday
                    timeRange.To = timeRange.From.AddDays(7).AddTicks(-1); // Sunday
                    break;

                case DashboardTimeRange.Month:
                    timeRange.From = new DateTime(today.Year, today.Month, 1);
                    timeRange.To = timeRange.From.AddMonths(1).AddTicks(-1);
                    break;

                case DashboardTimeRange.All:
                    timeRange.From = DateTime.MinValue;
                    timeRange.To = DateTime.MaxValue;
                    break;
            }

            // Lấy dữ liệu báo cáo
            GetMosByTimeRange(timeRange);
            dashboardData.MOReport =  GetMOReportAsync();
            dashboardData.MaterialReport = GetMaterialReportDTOAsync();
            dashboardData.RawMaterialReport = await GetRawMaterialDTOAsync();
            dashboardData.OeeReport = await GetOeeReportAsync();
            dashboardData.MachineReport = await GetMachineReportAsync();
            dashboardData.ErrorReport = await GetErrorReportAsync();

            return dashboardData;
        }

        private class TimeRange
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }

        private void GetMosByTimeRange(TimeRange timeRange)
        {
            MOs =  _moRepository.GetMOByTime(timeRange.From, timeRange.To).Result ?? new List<ManufacturingOrder>();
            mos = MOs.Where(x => x.Status == EStatus.Completed).ToList();
        }

        private MOReportDTO GetMOReportAsync()
        {
            var totalMOs = MOs.Count;
            var completedMOs = MOs.Count(mo => mo.Status == EStatus.Completed);
            var inProgressMOs = MOs.Count(mo => mo.Status == EStatus.InProduction);
            var pendingMOs = MOs.Count(mo => mo.Status == EStatus.Pending);
            var pendingScheduledMOs = MOs.Count(mo => mo.Status == EStatus.PendingScheduling);
            var readyToProduceMOs = MOs.Count(mo => mo.Status == EStatus.ReadyForProduction);
            var cancelledMOs = MOs.Count(mo => mo.Status == EStatus.Cancelled);
            double completionRate = totalMOs > 0 ? (double)completedMOs / totalMOs * 100 : 0;

            var moReport = new MOReportDTO
            {
                TotalMOs = totalMOs,
                PendingMOs = pendingMOs,
                PendingScheduledMOs = pendingScheduledMOs,
                ReadyToProduceMOs = readyToProduceMOs,
                InProgressMOs = inProgressMOs,
                CompletedMOs = completedMOs,
                CancelledMOs = cancelledMOs,
                CompletionRate = Math.Round(completionRate,2)
            };

            return moReport;
        }

        private MaterialReportDTO GetMaterialReportDTOAsync()
        {
            if (!mos.Any())
            {
                return new MaterialReportDTO
                {
                    TotalFinishedMaterials = 0,
                    CuFinishedMaterials = 0,
                    AlFinishedMaterials = 0,
                };
            }

            var totalFinishedMaterials = ((double)mos.Sum(mo => mo.Quantity));
            var cuFinishedMaterials = 0.0;
            var alFinishedMaterials = 0.0;
            foreach (var mo in mos)
            {
                string GetStringProperty(string propertyName) =>
                    mo.MaterialDefinition.Properties.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";

                if (GetStringProperty("Type") == "Cu")
                {
                    cuFinishedMaterials += (double)mo.Quantity;
                }
                else if (GetStringProperty("Type") == "Al")
                {
                    alFinishedMaterials += (double)mo.Quantity;
                }
            }
            
            cuFinishedMaterials = cuFinishedMaterials * 100 / totalFinishedMaterials;
            alFinishedMaterials = alFinishedMaterials * 100 / totalFinishedMaterials;

            // Implement logic to fetch and calculate MO report data based on the time range
            var MaterialReport = new MaterialReportDTO
            {
                TotalFinishedMaterials = totalFinishedMaterials,
                CuFinishedMaterials = Math.Round(cuFinishedMaterials,2),
                AlFinishedMaterials = Math.Round(alFinishedMaterials,2)

            };

            return MaterialReport;
        }

        private async Task<RawMaterialDTO> GetRawMaterialDTOAsync()
        {
            var totalMOs = mos.Count;
            double totalRawMaterials = 0.0;
            if (!mos.Any())
            {
                return new RawMaterialDTO
                {
                    TotalRawMaterial = totalRawMaterials,
                    RawMaterialsInMOs = new List<ListRawMaterialInMO>()
                };
            }

            var rawMaterialsInMOs = new List<ListRawMaterialInMO>();

            foreach (var mo in mos)
            {
                totalRawMaterials += mo.WorkOrders
                    .SelectMany(wo => wo.MaterialWorkOrders)
                    .Sum(mwo => (double)mwo.Quantity.GetValue());

                foreach (var wo in mo.WorkOrders)
                {
                    foreach(var item in wo.MaterialWorkOrders)
                    {
                        var existingRawMaterial = rawMaterialsInMOs.FirstOrDefault(rm => rm.MaterialId == item.MaterialDefinition.ResourceId);
                        if (existingRawMaterial != null)
                        {
                            existingRawMaterial.Quantity += (double)item.Quantity.GetValue();
                            existingRawMaterial.RateInMO = existingRawMaterial.Quantity * 100 / totalMOs;
                        }
                        else
                        {
                            rawMaterialsInMOs.Add(new ListRawMaterialInMO
                            {
                                MaterialId = item.MaterialDefinition.ResourceId,
                                MaterialName = await _materialDefinitionRepository.GetNameByIdAsync(item.MaterialDefinition.ResourceId),
                                Quantity = (double)item.Quantity.GetValue(),
                                RateInMO = (double)item.Quantity.GetValue() * 100 / totalRawMaterials,
                                Unit = "Kg"
                            });
                        }
                    }                    
                }
            }

            foreach (var rawMaterial in rawMaterialsInMOs)
            {
                rawMaterial.RateInMO = Math.Round(rawMaterial.RateInMO, 2);
            }

            var rawMaterialReport = new RawMaterialDTO
            {
                TotalRawMaterial = totalRawMaterials,
                RawMaterialsInMOs = rawMaterialsInMOs
            };

            return rawMaterialReport;
        }

        private async Task<OeeDTO> GetOeeReportAsync()
        {

            // Calculate Availability
            double totalRunTime = 0;
            double totalPlannedProductionTime = 0;
            foreach (var mo in mos)
            {
                foreach (var wo in mo.WorkOrders)
                {
                    totalRunTime += wo.RunTime;
                    totalPlannedProductionTime += wo.Duration;
                }
            }
            var availability = totalPlannedProductionTime > 0 ? (totalRunTime / totalPlannedProductionTime) : 0;
            
            // Calculate Performance
            double totalIdealRunTime = 0;
            double totalActuallyRunTime = 0;
            foreach (var mo in mos)
            {
                foreach (var wo in mo.WorkOrders)
                {
                    totalIdealRunTime += (wo.EndTime.Value - wo.StartTime.Value).TotalHours;
                    var actuallyRunTime = wo.ActuallyEndTime.HasValue && wo.ActuallyStartTime.HasValue
                        ? (wo.ActuallyEndTime.Value - wo.ActuallyStartTime.Value).TotalHours
                        : 0;
                    totalActuallyRunTime += (actuallyRunTime - wo.SetupTime - wo.TeardownTime);
                }
            }
            var performance = totalActuallyRunTime > 0 ? (totalIdealRunTime / totalActuallyRunTime) : 0;

            // Calculate Quality
            double totalProduced = 0;
            double totalTarget = 0;
            foreach (var mo in mos)
            {
                foreach (var wo in mo.WorkOrders)
                {
                    var progressRecords = await _workOrderProgressRecordRepository.GetByWorkOrderId(wo.WorkOrderId);
                    var progressRecord = progressRecords.FirstOrDefault(x => x.Status == EStatus.Completed);
                    totalProduced += progressRecord?.ProducedQuantity?? 0;
                    totalTarget += progressRecord?.TotalQuantity?? 0;
                }
            }
            var quality = totalTarget > 0 ? (totalProduced / totalTarget) : 0;
            
            var oee = availability * performance * quality;

            // Implement logic to fetch and calculate OEE report data based on the time range
            var oeeReport = new OeeDTO
            {
                OEE = Math.Round(oee * 100,2),
                Availability = Math.Round(availability * 100,2),
                Performance = Math.Round(performance * 100, 2),
                Quality = Math.Round(quality * 100, 2)
            };

            return oeeReport;
        }

        private async Task<MachineReportDTO> GetMachineReportAsync()
        {
            var drawMachines = new List<DrawMachineDTO>();
            var enamelMachines = new List<EnamelMachineDTO>();
            var drawEnamelMachines = new List<DrawEnamelMachineDTO>();
            var wos = mos.SelectMany(mo => mo.WorkOrders).ToList();
            var equipments = await _equipmentRepository.GetAll();
            
            foreach (var wo in wos)
            {
                var workOrder = await _workOrderRepository.GetByWoIdAsync(wo.WorkOrderId);

                foreach (var eq in workOrder?.EquipmentWorkOrders ?? new List<EquipmentWorkOrder>())
                {
                    var equipment = equipments.FirstOrDefault(e => e.ResourceId == eq.Equipment.ResourceId);
                    if (equipment is not null)
                    {
                        switch (equipment.EquipmentClass.ResourceId)
                        {
                            case "EC01":
                                var existingEquipment = drawMachines.FirstOrDefault(rm => rm.MachineId == equipment.ResourceId);
                                if (existingEquipment != null)
                                {
                                    existingEquipment.TotalDraws += ((double)wo.ProductionTarget);
                                }   
                                else
                                {
                                    drawMachines.Add(new DrawMachineDTO
                                    {
                                        MachineId = equipment.ResourceId,
                                        MachineName = equipment.Name,
                                        TotalDraws = ((double)wo.ProductionTarget)
                                    });
                                }
                                break;

                            case "EC02":
                                var existingEnamelEquipment = enamelMachines.FirstOrDefault(rm => rm.MachineId == equipment.ResourceId);
                                if (existingEnamelEquipment != null)
                                {
                                    existingEnamelEquipment.TotalEnamels += ((double)wo.ProductionTarget);
                                }   
                                else
                                {
                                    enamelMachines.Add(new EnamelMachineDTO
                                    {
                                        MachineId = equipment.ResourceId,
                                        MachineName = equipment.Name,
                                        TotalEnamels = ((double)wo.ProductionTarget)
                                    });
                                }
                                break;

                            case "EC03":
                                var existingDrawEnamelEquipment = drawEnamelMachines.FirstOrDefault(rm => rm.MachineId == equipment.ResourceId);
                                if (existingDrawEnamelEquipment != null)
                                {
                                    existingDrawEnamelEquipment.TotalDrawEnamels += ((double)wo.ProductionTarget);
                                }   
                                else
                                {
                                    drawEnamelMachines.Add(new DrawEnamelMachineDTO
                                    {
                                        MachineId = equipment.ResourceId,
                                        MachineName = equipment.Name,
                                        TotalDrawEnamels = ((double)wo.ProductionTarget)
                                    });
                                }
                                break;
                        }
                    }
                }


            }

            foreach (var machine in drawMachines)
            {
                machine.TotalDraws = Math.Round(machine.TotalDraws, 2);
            }

            foreach (var machine in enamelMachines)
            {
                machine.TotalEnamels = Math.Round(machine.TotalEnamels, 2);
            }

            foreach (var machine in drawEnamelMachines)
            {
                machine.TotalDrawEnamels = Math.Round(machine.TotalDrawEnamels, 2);
            }

            var machineReport = new MachineReportDTO
            {
                TotalWorkOrders = wos.Count,
                TotalMachines = drawMachines.Count + enamelMachines.Count + drawEnamelMachines.Count,
                DrawMachines = drawMachines,
                EnamelMachines = enamelMachines,
                DrawEnamelMachines = drawEnamelMachines
            };

            return machineReport;
        }

        private async Task<ErrorReportDTO> GetErrorReportAsync()
        {
            var errors = await _workOrderErrorRecordRepository.GetAllAsync();

            var newErrors = errors.Where(x => x.End != DateTime.MinValue).ToList();

            var ErrorsInMOs = new List<ListErrorInMO>();

            foreach (var er in newErrors)
            {
                var workOrder = await _workOrderRepository.GetByWoIdAsync(er.WorkOrderId);
                if (workOrder is not null)
                {
                    var mo = MOs.FirstOrDefault(mo => mo.WorkOrders.Any(wo => wo.WorkOrderId == workOrder.WorkOrderId));
                    if (mo is not null)
                    {
                        var existingError = ErrorsInMOs.FirstOrDefault(e => e.ErrorType == er.DowntimeType.ToString());
                        if (existingError != null)
                        {
                            existingError.ErrorCount += 1;
                        }
                        else
                        {
                            ErrorsInMOs.Add(new ListErrorInMO
                            {
                                MOId = mo.ManufacturingOrderId,
                                WOId = workOrder.WorkOrderId,
                                ErrorType = er.DowntimeType.ToString(),
                                ErrorCount = 1
                            });
                        }
                    }
                }
            }

            var errorReport = new ErrorReportDTO { ErrorsInMOs = ErrorsInMOs, TotalMOs = MOs.Count };


            return errorReport;
        }
    }
}
