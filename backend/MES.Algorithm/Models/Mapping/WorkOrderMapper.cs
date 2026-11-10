using MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MesMicroservice.Algorithm.Models.Mapping
{
    public static class WorkOrderMapper
    {
        public static Job ToJob(
            WorkOrder workOrder,
            double inputMaterialDiameter,
            string equipmentType,
            double speed)
        {
            var props = workOrder.MaterialDefinition?.Properties
                ?? new List<MaterialDefinitionProperty>();

            double GetDouble(string name) =>
                double.TryParse(
                    props.FirstOrDefault(p =>
                        p.Name.Equals(
                            name,
                            StringComparison.OrdinalIgnoreCase))
                    ?.Value?.ValueString,
                    out var v)
                    ? v
                    : 0;

            var materialSpecInfors = workOrder.OperationsSegment
                .MaterialSpecifications
                .Select(x => x.MaterialDefinitionId)
                .Where(id => id != null)
                .ToList();

            DateTime schedulingStartTime = DateTime.Now;
            double calculatedDueDateHours = 168.0;

            if (workOrder.DueDate > DateTime.MinValue)
            {
                calculatedDueDateHours = Math.Max(
                    1,
                    (workOrder.DueDate - schedulingStartTime).TotalHours);
            }

            return new Job
            {
                Id = workOrder.Id,
                Name = workOrder.OperationsSegment.Description,
                ProcessingTime = workOrder.RunTime,
                ConfiguredProcessingTimeHours = MesMicroservice.Algorithm.Services.SegmentDuration.GetHours(
                    workOrder.OperationsSegment.SegmentType,
                    workOrder.OperationsSegment.Duration?.ValueString,
                    workOrder.OperationsSegment.ValueUnitOfMeasure,
                    workOrder.ProductionTarget),
                SetupTime = workOrder.SetupTime,
                TeardownTime = workOrder.TeardownTime,
                TotalTime = 0,

                DueDate = calculatedDueDateHours,
                Priority = workOrder.ManufacturingOrder?.Priority ?? 1,
                AssignedMachineCode = "",
                ManufacturingOrderId =
                    workOrder.ManufacturingOrder?.ManufacturingOrderId ?? "",
                SequenceNumber = workOrder.SequenceNumber,
                Quatity = workOrder.ProductionTarget,
                Price = GetDouble("price"),
                Diameter = GetDouble("K/C"),
                Density = GetDouble("density"),
                RetireLayers =
                    workOrder.OperationsSegment.MaterialSpecifications.Count(),
                MaterialDefinitionId =
                    workOrder.MaterialDefinition?.ResourceId ?? "",
                Type = props.FirstOrDefault(p =>
                        p.Name.Equals(
                            "Type",
                            StringComparison.OrdinalIgnoreCase))
                    ?.Value?.ValueString ?? "",
                MaterialSpecInfors = materialSpecInfors,

                // [ĐÃ BỎ]
                // SegmentType = workOrder.OperationsSegment.SegmentType,
                //
                // Algorithm không cần biết công đoạn là
                // Drawing / Enameling / Both nữa.

                IndexInSchedule = 0,
                InputMaterialDiameter = inputMaterialDiameter,
                EquipmentType = equipmentType,
                SpeedRetirement = speed
            };
        }
    }
}
