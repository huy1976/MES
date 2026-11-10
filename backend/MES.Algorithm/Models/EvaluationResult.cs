using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Algorithm.Models
{
    public class EvaluationResult
    {
        public string AlgorithmName { get; set; }
        public double Makespan { get; set; }
        public double AverageLateness { get; set; }
        public double TotalSetupTime { get; set; }
        public TimeSpan ComputationTime { get; set; }
    }

}
