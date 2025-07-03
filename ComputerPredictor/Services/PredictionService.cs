using ComputerPredictor.Models;

namespace ComputerPredictor.Services;

public sealed class PredictionService
{
    private readonly Dictionary<string, Func<Features, ValueTask<object>>> _predictors;

    public PredictionService()
    {
        _predictors = new(StringComparer.OrdinalIgnoreCase)
        {
            ["cpu_cores"] = features => PredictCpuCoresAsync(features).AsValueTaskOfObject(),
            ["cpu_speed_ghz"] = features => PredictCpuSpeedGhzAsync(features).AsValueTaskOfObject(),
            ["ram_gb"] = features => PredictRamGbAsync(features).AsValueTaskOfObject(),
            ["storage_gb"] = features => PredictStorageGbAsync(features).AsValueTaskOfObject(),
            ["storage_type"] = features => PredictStorageTypeAsync(features).AsValueTaskOfObject(),
            ["operating_system"] = features => PredictOperatingSystemAsync(features).AsValueTaskOfObject(),
            ["target_use_case"] = features => PredictTargetUseCaseAsync(features).AsValueTaskOfObject(),
            ["price_usd"] = features => PredictPriceUsdAsync(features).AsValueTaskOfObject(),
        };
    }

    public ValueTask<object> PredictAsync(string target, Features features)
    {
        if (!_predictors.TryGetValue(target, out var predictor))
            throw new ArgumentException($"Unknown target: {target}");

        return predictor(features);
    }

    private ValueTask<float> PredictCpuCoresAsync(Features features) =>
        ValueTask.FromResult(CpuCores.Predict(new CpuCores.ModelInput
        {
            Cpu_speed_ghz = (float)features.CpuSpeedGhz!,
            Cpu_cores = 0,
            Ram_gb = (float)features.RamGb!,
            Storage_type = features.StorageType!,
            Storage_gb = (float)features.StorageGb!,
            Operating_system = features.OperatingSystem!,
            Target_use_case = features.TargetUseCase!,
            Price_usd = (float)features.PriceUsd!
        }).PredictedLabel);

    private ValueTask<float> PredictCpuSpeedGhzAsync(Features features) =>
        ValueTask.FromResult(CpuSpeedGhz.Predict(new CpuSpeedGhz.ModelInput
        {
            Cpu_speed_ghz = 0,
            Cpu_cores = (float)features.CpuCores!,
            Ram_gb = (float)features.RamGb!,
            Storage_type = features.StorageType!,
            Storage_gb = (float)features.StorageGb!,
            Operating_system = features.OperatingSystem!,
            Target_use_case = features.TargetUseCase!,
            Price_usd = (float)features.PriceUsd!
        }).Score);

    private ValueTask<float> PredictRamGbAsync(Features features) =>
        ValueTask.FromResult(RamGb.Predict(new RamGb.ModelInput
        {
            Cpu_speed_ghz = (float)features.CpuSpeedGhz!,
            Cpu_cores = (float)features.CpuCores!,
            Ram_gb = 0,
            Storage_type = features.StorageType!,
            Storage_gb = (float)features.StorageGb!,
            Operating_system = features.OperatingSystem!,
            Target_use_case = features.TargetUseCase!,
            Price_usd = (float)features.PriceUsd!
        }).PredictedLabel);

    private ValueTask<float> PredictStorageGbAsync(Features features) =>
        ValueTask.FromResult(StorageGb.Predict(new StorageGb.ModelInput
        {
            Cpu_speed_ghz = (float)features.CpuSpeedGhz!,
            Cpu_cores = (float)features.CpuCores!,
            Ram_gb = (float)features.RamGb!,
            Storage_type = features.StorageType!,
            Storage_gb = 0,
            Operating_system = features.OperatingSystem!,
            Target_use_case = features.TargetUseCase!,
            Price_usd = (float)features.PriceUsd!
        }).PredictedLabel);

    private ValueTask<string> PredictStorageTypeAsync(Features features) =>
        ValueTask.FromResult(StorageType.Predict(new StorageType.ModelInput
        {
            Cpu_speed_ghz = (float)features.CpuSpeedGhz!,
            Cpu_cores = (float)features.CpuCores!,
            Ram_gb = (float)features.RamGb!,
            Storage_type = null!,
            Storage_gb = (float)features.StorageGb!,
            Operating_system = features.OperatingSystem!,
            Target_use_case = features.TargetUseCase!,
            Price_usd = (float)features.PriceUsd!
        }).PredictedLabel);

    private ValueTask<string> PredictOperatingSystemAsync(Features features) =>
        ValueTask.FromResult(OperatingSystem.Predict(new OperatingSystem.ModelInput
        {
            Cpu_speed_ghz = (float)features.CpuSpeedGhz!,
            Cpu_cores = (float)features.CpuCores!,
            Ram_gb = (float)features.RamGb!,
            Storage_type = features.StorageType!,
            Storage_gb = (float)features.StorageGb!,
            Operating_system = null!,
            Target_use_case = features.TargetUseCase!,
            Price_usd = (float)features.PriceUsd!
        }).PredictedLabel);

    private ValueTask<string> PredictTargetUseCaseAsync(Features features) =>
        ValueTask.FromResult(TargetUseCase.Predict(new TargetUseCase.ModelInput
        {
            Cpu_speed_ghz = (float)features.CpuSpeedGhz!,
            Cpu_cores = (float)features.CpuCores!,
            Ram_gb = (float)features.RamGb!,
            Storage_type = features.StorageType!,
            Storage_gb = (float)features.StorageGb!,
            Operating_system = features.OperatingSystem!,
            Target_use_case = null!,
            Price_usd = (float)features.PriceUsd!
        }).PredictedLabel);

    private ValueTask<float> PredictPriceUsdAsync(Features features) =>
        ValueTask.FromResult(PriceUsd.Predict(new PriceUsd.ModelInput
        {
            Cpu_speed_ghz = (float)features.CpuSpeedGhz!,
            Cpu_cores = (float)features.CpuCores!,
            Ram_gb = (float)features.RamGb!,
            Storage_type = features.StorageType!,
            Storage_gb = (float)features.StorageGb!,
            Operating_system = features.OperatingSystem!,
            Target_use_case = features.TargetUseCase!,
            Price_usd = 0
        }).Score);
}

public static class ValueTaskExtensions
{
    public static ValueTask<object> AsValueTaskOfObject<T>(this ValueTask<T> task) =>
        task.IsCompletedSuccessfully ? new ValueTask<object>(task.Result!) : BoxAsync(task);

    private static async ValueTask<object> BoxAsync<T>(ValueTask<T> task) =>
        await task;
}
