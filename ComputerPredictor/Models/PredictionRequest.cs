using ComputerPredictor.Models;

namespace ComputerPredictor.Models
{
    public sealed class PredictionRequest
    {
        public string Target { get; set; } = default!;
        public Features Features { get; set; } = default!;
    }
}