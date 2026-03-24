using System;
using System.Threading.Tasks;

namespace AvaloniaExercise.Models;

public interface ITrafficLightService
{
    TrafficLightStatus Status { get; }

    bool IsCrossingRequested { get; }

    DateTime? CrossingTimeExpiryUtc { get; }

    event EventHandler<TrafficLightStatus>? StatusChanged;

    event EventHandler<bool>? IsCrossingRequestedChanged;

    event EventHandler<DateTime?>? CrossingTimeExpiryUtcChanged;

    Task RequestCrossingAsync();
}

public enum TrafficLightStatus
{
    Red,
    Amber,
    Green
}
