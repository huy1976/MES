global using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;

namespace MesMicroservice.Algorithm.Models.Mapping
{
    public static class MachineMapper
    {
        public static Machine FromEquipment(Equipment e, double availableTime)
        {
            var eqcapproperties = e.EquipmentCapabilities?
                .SelectMany(x => x.Properties)
                .ToList()
                ?? new List<EquipmentCapabilityProperty>();

            double GetDoubleCapability(string name) =>
                double.TryParse(eqcapproperties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value?.ValueString,
                    out var v) ? v : 0;

            //string GetStringCapability(string name) =>
            //    eqcapproperties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";

            var capabilities = eqcapproperties.Select(p => new MachineCapabilityProperty(
                name: p.Name,
                description: p.Description ?? "",
                value: GetDoubleCapability(p.Name),
                unit: p.ValueUnitOfMeasure ?? ""
            )).ToList();

            var eqproperties = e.Properties ?? new List<EquipmentProperty>();
            int GetIntProperty(string name) =>
                int.TryParse(eqproperties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value?.ValueString,
                    out var v) ? v : 0;

            string GetStringProperty(string name) =>
                eqproperties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";

            double GetDoubleProperty(string name) =>
                double.TryParse(eqproperties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value?.ValueString,
                    out var v) ? v : 0;

            return new Machine
            {
                Id = e.Id,
                Code = e.ResourceId ?? e.Name ?? $"M-{e.Id}",
                WorkCenterCode = e.HierarchyModel?.Parent?.HierarchyModelId ?? "",
                ProcessingTime = 0,
                TotalTime = 0,
                ClassId = e.EquipmentClass.ResourceId ?? "",
                Properties = new MachineProperties
                {
                    Speed = GetDoubleProperty("speed"),
                    Efficiency = GetDoubleProperty("efficiency"),
                    SetupTimeMin = (GetDoubleCapability("setup_time_min"))/60.0,
                    SetupTimeMax = (GetDoubleCapability("setup_time_max"))/60.0,
                    DryTime = (GetDoubleCapability("dry_time_min")) / 60.0,
                    AnnealDwellTime = GetDoubleCapability("anneal_dwell_time_s"),
                    Heads = GetIntProperty("head"),
                    Lines = GetIntProperty("lines"),
                    Layers = GetIntProperty("layerSupported"),
                    DryType = GetStringProperty("dryType"),
                    MinDiameter = GetDoubleProperty("min-diameter"),
                    MaxDiameter = GetDoubleProperty("max-diameter"),
                    InputMin = GetDoubleProperty("inputMin"),
                    InputMax = GetDoubleProperty("inputMax"),
                    OvenLength = GetDoubleProperty("ovenLength"),
                    Priority = GetDoubleCapability("priority")
                },
                Capabilities = capabilities,
                AvailableTime = availableTime

            };
        }

    }
}
