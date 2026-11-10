using MesMicroservice.Algorithm.Models;
using MesMicroservice.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MesMicroservice.Algorithm.Services
{
    public static class AIFeatureExtractor
    {
        public static TabuAITrainingData Extract(List<Job> jobs, List<Machine> machines)
        {
            if (jobs == null || !jobs.Any())
                return new TabuAITrainingData();

            machines ??= new List<Machine>();

            var orderedJobs = jobs
                .OrderBy(j => j.IndexInSchedule)
                .ThenBy(j => j.Id)
                .ToList();

            int totalJobs = orderedJobs.Count;
            int machineCount = Math.Max(1, machines.Count);

            double avgProcessing = orderedJobs.Average(j => j.ProcessingTime);
            double avgDiameter = orderedJobs.Average(j => j.Diameter);
            double avgSetup = orderedJobs.Average(j => j.SetupTime);

            var features = new TabuAITrainingData
            {
                TotalJobs = totalJobs,
                MachineCount = machineCount,

                AvgEligibleMachines = (float)orderedJobs.Average(j => j.EligibleMachines?.Count ?? 1),
                MinEligibleMachines = orderedJobs.Min(j => j.EligibleMachines?.Count ?? 1),
                MaxEligibleMachines = orderedJobs.Max(j => j.EligibleMachines?.Count ?? 1),

                AvgProcessingTime = (float)avgProcessing,
                StdProcessingTime = totalJobs > 1
                    ? (float)Math.Sqrt(orderedJobs.Average(j => Math.Pow(j.ProcessingTime - avgProcessing, 2)))
                    : 0,

                AvgDiameter = (float)avgDiameter,
                DiameterVariance = totalJobs > 1
                    ? (float)orderedJobs.Average(j => Math.Pow(j.Diameter - avgDiameter, 2))
                    : 0,

                AvgSetupTime = (float)avgSetup,
                StdSetupTime = totalJobs > 1
                    ? (float)Math.Sqrt(orderedJobs.Average(j => Math.Pow(j.SetupTime - avgSetup, 2)))
                    : 0,

                InitialMakespan = 0,
                InitialObjective = 0
            };

            int setupConflict = 0;
            int sameMaterial = 0;
            int sameDiameter = 0;

            for (int i = 0; i < orderedJobs.Count - 1; i++)
            {
                var a = orderedJobs[i];
                var b = orderedJobs[i + 1];

                bool materialSame = a.MaterialDefinitionId == b.MaterialDefinitionId;
                bool diameterSame = Math.Abs(a.Diameter - b.Diameter) < 0.01;

                if (!materialSame)
                    setupConflict++;
                else
                    sameMaterial++;

                if (diameterSame)
                    sameDiameter++;
            }

            int transitions = Math.Max(1, totalJobs - 1);
            features.SetupConflictRatio = (float)setupConflict / transitions;
            features.SameMaterialRatio = (float)sameMaterial / transitions;
            features.SameDiameterRatio = (float)sameDiameter / transitions;

            var machineSpeeds = machines
                .Select(m => m.Properties?.Speed ?? 0)
                .Where(x => x > 0)
                .ToList();

            if (machineSpeeds.Count > 1)
            {
                double avgSpeed = machineSpeeds.Average();
                features.MachineSpeedStd =
                    (float)Math.Sqrt(machineSpeeds.Average(x => Math.Pow(x - avgSpeed, 2)));
            }

            var machineEfficiencies = machines
                .Select(m => m.Properties?.Efficiency ?? 0)
                .Where(x => x > 0)
                .ToList();

            if (machineEfficiencies.Count > 1)
            {
                double avgEff = machineEfficiencies.Average();
                features.MachineEfficiencyStd =
                    (float)Math.Sqrt(machineEfficiencies.Average(x => Math.Pow(x - avgEff, 2)));
            }

            features.MachineCapabilityImbalance = CalculateMachineCapabilityImbalance(orderedJobs, machines);

            features.AssignmentSensitivity =
                features.SetupConflictRatio * (1f / Math.Max(1f, features.AvgEligibleMachines));

            features.MachinePreferenceSharpness =
                features.MachineSpeedStd / Math.Max(1f, features.AvgProcessingTime);

            features.BottleneckLoadRatio =
                features.SetupConflictRatio * features.AssignmentSensitivity;

            features.MachineLoadStd = CalculateMachineLoadStd(orderedJobs, machines);

            features.BottleneckSensitivity =
                features.BottleneckLoadRatio * features.MachinePreferenceSharpness;

            FillDueDateFeatures(features, orderedJobs);

            return features;
        }

        private static float CalculateMachineCapabilityImbalance(List<Job> jobs, List<Machine> machines)
        {
            if (machines == null || machines.Count <= 1)
                return 0;

            var loads = machines
                .Select(m => jobs.Count(j =>
                    j.EligibleMachines != null &&
                    j.EligibleMachines.Any(em => em.Code == m.Code || em.Id == m.Id)))
                .Select(x => (double)x)
                .ToList();

            if (loads.Count <= 1)
                return 0;

            double avg = loads.Average();
            return (float)Math.Sqrt(loads.Average(x => Math.Pow(x - avg, 2)));
        }

        private static float CalculateMachineLoadStd(List<Job> jobs, List<Machine> machines)
        {
            if (machines == null || machines.Count <= 1)
                return 0;

            var loads = machines
                .Select(m => jobs
                    .Where(j => j.AssignedMachineCode == m.Code)
                    .Sum(j => j.ProcessingTime + j.SetupTime))
                .ToList();

            if (loads.Count <= 1)
                return 0;

            double avg = loads.Average();
            return (float)Math.Sqrt(loads.Average(x => Math.Pow(x - avg, 2)));
        }

        private static void FillDueDateFeatures(TabuAITrainingData features, List<Job> jobs)
        {
            var jobsWithDueDate = jobs
                .Where(j => j.DueDate > 0)
                .ToList();

            if (!jobsWithDueDate.Any())
            {
                features.TightDueDateRatio = 0;
                features.AvgSlackTime = 0;
                features.CriticalJobRatio = 0;
                return;
            }

            var slacks = jobsWithDueDate
                .Select(j =>
                {
                    double referenceTime = j.EndTime > 0 ? j.EndTime : j.ProcessingTime;
                    return j.DueDate - referenceTime;
                })
                .ToList();

            features.AvgSlackTime = (float)slacks.Average();
            features.TightDueDateRatio = (float)slacks.Count(s => s <= 24) / jobsWithDueDate.Count;
            features.CriticalJobRatio = (float)slacks.Count(s => s < 0) / jobsWithDueDate.Count;
        }
    }
}