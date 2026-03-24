using System;
using System.Collections.Generic;

namespace AvaloniaExercise.Models;

public interface IPedestrianSensorService
{
    IEnumerable<Pedestrian> Pedestrians { get; }

    event EventHandler<PedestriansChangedEventArgs>? PedestriansChanged;
}

public class PedestriansChangedEventArgs : EventArgs
{
    public required Pedestrian Pedestrian { get; init; }

    public required PedestrianOperation Operation { get; init; }
}

public enum PedestrianOperation
{
    Arrived,
    Left
}
