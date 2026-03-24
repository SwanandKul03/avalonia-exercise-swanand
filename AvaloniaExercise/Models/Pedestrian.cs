using System;
using System.Drawing;
using AvaloniaExercise.Models.Impl;

namespace AvaloniaExercise.Models;

public class Pedestrian
{
    /// <summary>
    /// Unique ID for this Pedestrian
    /// </summary>
    public Guid Id { get; } = Guid.CreateVersion7();

    public required string Name { get; init; }

    public required PedestrianSpecies Species { get; init; }

    public required Color ShirtColor { get; init; }

    public required Color ShortsColor { get; init; }

    public DateTime ArrivedAtUtc { get; init; }

    public PedestrianStatus Status { get; private set; } = PedestrianStatus.WaitingToCross;


    public event EventHandler<PedestrianStatus>? StatusChanged;


    public override string ToString() => $"{Name} ({Species.ToDisplayName()})";

    internal void UpdateStatus(PedestrianStatus status)
    {
        if (Status != status)
        {
            Status = status;
            StatusChanged?.Invoke(this, status);
        }
    }
}

public enum PedestrianStatus
{
    WaitingToCross,
    Crossing,
    Crossed
}
