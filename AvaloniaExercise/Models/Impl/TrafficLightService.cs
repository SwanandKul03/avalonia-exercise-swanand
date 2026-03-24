using System;
using System.Threading.Tasks;

namespace AvaloniaExercise.Models.Impl;

public class TrafficLightService : ITrafficLightService
{
    public const int RequestedAmberTimeSeconds = 5;
    public const int RedCrossingTimeSeconds = 15;
    public const int CooldownAmberTimeSeconds = 5;

    private const int RequestedAmberTimeMs = RequestedAmberTimeSeconds * 1000;
    private const int RedCrossingTimeMs = RedCrossingTimeSeconds * 1000;
    private const int CooldownAmberTimeMs = CooldownAmberTimeSeconds * 1000;

    private readonly PedestrianSensorService _pedestrianSensorService;

    public TrafficLightService(PedestrianSensorService pedestrianSensorService)
    {
        _pedestrianSensorService = pedestrianSensorService;
    }

    public TrafficLightStatus Status { get; private set; } = TrafficLightStatus.Green;

    public bool IsCrossingRequested { get; private set; }

    public DateTime? CrossingTimeExpiryUtc { get; private set; }

    public event EventHandler<TrafficLightStatus>? StatusChanged;

    public event EventHandler<bool>? IsCrossingRequestedChanged;

    public event EventHandler<DateTime?>? CrossingTimeExpiryUtcChanged;

    public async Task RequestCrossingAsync()
    {
        if (IsCrossingRequested && Status != TrafficLightStatus.Green)
            return;

        UpdateIsCrossingRequested(true);

        UpdateStatus(TrafficLightStatus.Amber);
        await Task.Delay(RequestedAmberTimeMs).ConfigureAwait(false);
        UpdateStatus(TrafficLightStatus.Red);
        UpdateIsCrossingRequested(false);

        _pedestrianSensorService.SetAllWaitingPedestriansCrossing();

        UpdateCrossingTimeExpiryUtc(DateTime.UtcNow + TimeSpan.FromMilliseconds(RedCrossingTimeMs));
        await Task.Delay(RedCrossingTimeMs).ConfigureAwait(false);
        UpdateCrossingTimeExpiryUtc(null);

        _pedestrianSensorService.SetAllCrossingPedestriansCrossed();

        UpdateStatus(TrafficLightStatus.Amber);
        await Task.Delay(CooldownAmberTimeMs).ConfigureAwait(false);
        UpdateStatus(TrafficLightStatus.Green);
    }

    private void UpdateIsCrossingRequested(bool isCrossingRequested)
    {
        if (IsCrossingRequested != isCrossingRequested)
        {
            IsCrossingRequested = isCrossingRequested;
            IsCrossingRequestedChanged?.Invoke(this, IsCrossingRequested);
        }
    }

    private void UpdateStatus(TrafficLightStatus status)
    {
        if (Status != status)
        {
            Status = status;
            StatusChanged?.Invoke(this, Status);
        }
    }

    private void UpdateCrossingTimeExpiryUtc(DateTime? time)
    {
        if (CrossingTimeExpiryUtc != time)
        {
            CrossingTimeExpiryUtc = time;
            CrossingTimeExpiryUtcChanged?.Invoke(this, CrossingTimeExpiryUtc);
        }
    }
}
