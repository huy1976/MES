
using Microsoft.ML.Data;

namespace MesMicroservice.MachineLearning
{
    public class TabuAITrainingData
    {
        [LoadColumn(0)]
        public float TotalJobs { get; set; }

        [LoadColumn(1)]
        public float MachineCount { get; set; }

        [LoadColumn(2)]
        public float AvgEligibleMachines { get; set; }

        [LoadColumn(3)]
        public float MinEligibleMachines { get; set; }

        [LoadColumn(4)]
        public float MaxEligibleMachines { get; set; }

        [LoadColumn(5)]
        public float AvgProcessingTime { get; set; }

        [LoadColumn(6)]
        public float StdProcessingTime { get; set; }

        [LoadColumn(7)]
        public float AvgDiameter { get; set; }

        [LoadColumn(8)]
        public float DiameterVariance { get; set; }

        [LoadColumn(9)]
        public float AvgSetupTime { get; set; }

        [LoadColumn(10)]
        public float StdSetupTime { get; set; }

        [LoadColumn(11)]
        public float SetupConflictRatio { get; set; }

        [LoadColumn(12)]
        public float SameMaterialRatio { get; set; }

        [LoadColumn(13)]
        public float SameDiameterRatio { get; set; }

        [LoadColumn(14)]
        public float MachineSpeedStd { get; set; }

        [LoadColumn(15)]
        public float MachineEfficiencyStd { get; set; }

        [LoadColumn(16)]
        public float MachineCapabilityImbalance { get; set; }

        [LoadColumn(17)]
        public float AssignmentSensitivity { get; set; }

        [LoadColumn(18)]
        public float MachinePreferenceSharpness { get; set; }

        [LoadColumn(19)]
        public float BottleneckLoadRatio { get; set; }

        [LoadColumn(20)]
        public float MachineLoadStd { get; set; }

        [LoadColumn(21)]
        public float BottleneckSensitivity { get; set; }

        [LoadColumn(22)]
        public float TightDueDateRatio { get; set; }

        [LoadColumn(23)]
        public float AvgSlackTime { get; set; }

        [LoadColumn(24)]
        public float CriticalJobRatio { get; set; }

        [LoadColumn(25)]
        public float InitialMakespan { get; set; }

        [LoadColumn(26)]
        public float InitialObjective { get; set; }

        [LoadColumn(27)]
        public float Neighbors { get; set; }

        [LoadColumn(28)]
        public float Tenure { get; set; }
        [LoadColumn(29)]
        public float RelativeObjective { get; set; }

    }
}

