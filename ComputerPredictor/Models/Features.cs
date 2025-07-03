using System.Text.Json.Serialization;

namespace ComputerPredictor.Models
{
    public sealed class Features
    {
        [JsonPropertyName("cpu_speed_ghz")]
        public double? CpuSpeedGhz { get; set; }

        [JsonPropertyName("cpu_cores")]
        public double? CpuCores { get; set; }

        [JsonPropertyName("ram_gb")]
        public double? RamGb { get; set; }

        [JsonPropertyName("storage_type")]
        public string? StorageType { get; set; }

        [JsonPropertyName("storage_gb")]
        public double? StorageGb { get; set; }

        [JsonPropertyName("operating_system")]
        public string? OperatingSystem { get; set; }

        [JsonPropertyName("target_use_case")]
        public string? TargetUseCase { get; set; }

        [JsonPropertyName("price_usd")]
        public double? PriceUsd { get; set; }
    }
}
