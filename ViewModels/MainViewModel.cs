using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RealTimePedestrianControlSystem.Models;

namespace RealTimePedestrianControlSystem.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private static readonly string[] Names =
    {
        "Mara Chen", "Tobias Wren", "Ines Okafor", "Leo Sørensen", "Priya Nair",
        "Carlos Muñoz", "Anya Petrov", "Kwame Asante", "Hana Yoshida", "Ravi Kapoor",
        "Elise Botha", "Dmitri Volkov", "Zara Ibrahim", "Felix Lindqvist", "Amara Diop"
    };
    private const string LeftSide = "Left";
    private const string RightSide = "Right";
    private static readonly string[] SpawnSides = { LeftSide, RightSide };
    private static readonly string[] Speeds = { "slow", "normal", "normal", "fast" };
    private static readonly string[] Ages = { "Child", "Adult", "Adult", "Adult", "Elderly" };
    private static readonly Dictionary<CrossingStateEnum, int> PhaseDurations = new()
    {
        { CrossingStateEnum.Idle, 0 },
        { CrossingStateEnum.Requested, 3 },
        { CrossingStateEnum.Active, 15 },
        { CrossingStateEnum.Cooldown, 5 }
    };

    private readonly Random _rng = new();
    private Timer? _phaseTimer;
    private Timer? _pedSpawnTimer;
    private Timer? _waitUpdateTimer;
    private Timer? _clockTimer;
    private Timer? _peakTimer;
    private int _pedIdCounter = 1;

    [ObservableProperty] private CrossingStateEnum _crossingState = CrossingStateEnum.Idle;
    [ObservableProperty] private int _countdown;
    [ObservableProperty] private int _totalCrossings = 147;
    [ObservableProperty] private double _avgWaitTime = 12.4;
    [ObservableProperty] private double _uptime = 99.7;
    [ObservableProperty] private bool _peakHourActive;
    [ObservableProperty] private string _currentTime = "";
    [ObservableProperty] private int _queueSize;
    [ObservableProperty] private int _crossingCount;
    [ObservableProperty] private int _pedsLeftCount;
    [ObservableProperty] private int _pedsRightCount;

    public ObservableCollection<Pedestrian> Pedestrians { get; } = new();
    public IEnumerable<Pedestrian> LeftSidePedestrians => Pedestrians.Where(p => p.Direction == LeftSide);
    public IEnumerable<Pedestrian> RightSidePedestrians => Pedestrians.Where(p => p.Direction == RightSide);

    public MainViewModel()
    {
        Pedestrians.CollectionChanged += OnPedestriansCollectionChanged;
        UpdateClock();
        _clockTimer = new Timer(_ => Avalonia.Threading.Dispatcher.UIThread.Post(UpdateClock), null, 0, 1000);
        _waitUpdateTimer = new Timer(_ => Avalonia.Threading.Dispatcher.UIThread.Post(UpdateWaitTimes), null, 1000, 1000);
        _peakTimer = new Timer(_ => Avalonia.Threading.Dispatcher.UIThread.Post(() => PeakHourActive = !PeakHourActive), null, 30000, 30000);
        StartPedestrianSpawner();
    }

    private void UpdateClock() => CurrentTime = DateTime.Now.ToString("HH:mm:ss");

    private void UpdatePedCounts()
    {
        var waiting = Pedestrians.Where(p => p.Status == PedestrianStatus.Waiting).ToList();
        var crossing = Pedestrians.Where(p => p.Status == PedestrianStatus.Crossing).ToList();
        QueueSize = waiting.Count;
        CrossingCount = crossing.Count;
        PedsLeftCount = Pedestrians.Count(p => p.Direction == LeftSide);
        PedsRightCount = Pedestrians.Count(p => p.Direction == RightSide);
    }

    private void OnPedestriansCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(LeftSidePedestrians));
        OnPropertyChanged(nameof(RightSidePedestrians));
    }

    partial void OnCrossingStateChanged(CrossingStateEnum value)
    {
        _phaseTimer?.Dispose();
        _phaseTimer = null;

        if (value == CrossingStateEnum.Idle)
        {
            Countdown = 0;
            StartPedestrianSpawner();
            return;
        }

        int duration = PhaseDurations[value];
        Countdown = duration;

        if (value == CrossingStateEnum.Active)
        {
            // Mark waiting → crossing
            foreach (var p in Pedestrians.Where(p => p.Status == PedestrianStatus.Waiting))
                p.Status = PedestrianStatus.Crossing;
            // Force UI refresh
            var snapshot = Pedestrians.ToList();
            Pedestrians.Clear();
            foreach (var p in snapshot) Pedestrians.Add(p);
            UpdatePedCounts();
        }

        _phaseTimer = new Timer(_ => Avalonia.Threading.Dispatcher.UIThread.Post(PhaseTick), null, 1000, 1000);
    }

    private void PhaseTick()
    {
        Countdown--;
        if (Countdown <= 0)
        {
            _phaseTimer?.Dispose();
            _phaseTimer = null;

            switch (CrossingState)
            {
                case CrossingStateEnum.Requested:
                    CrossingState = CrossingStateEnum.Active;
                    break;
                case CrossingStateEnum.Active:
                    int crossed = Pedestrians.Count(p => p.Status == PedestrianStatus.Crossing);
                    var remaining = Pedestrians.Where(p => p.Status != PedestrianStatus.Crossing).ToList();
                    Pedestrians.Clear();
                    foreach (var p in remaining) Pedestrians.Add(p);
                    TotalCrossings += crossed;
                    UpdatePedCounts();
                    CrossingState = CrossingStateEnum.Cooldown;
                    break;
                case CrossingStateEnum.Cooldown:
                    CrossingState = CrossingStateEnum.Idle;
                    break;
            }
        }
    }

    [RelayCommand]
    private void TriggerCrossing()
    {
        if (CrossingState == CrossingStateEnum.Idle)
            CrossingState = CrossingStateEnum.Requested;
    }

    private void StartPedestrianSpawner()
    {
        _pedSpawnTimer?.Dispose();
        int delay = PeakHourActive ? 1500 : 2500;
        _pedSpawnTimer = new Timer(_ => Avalonia.Threading.Dispatcher.UIThread.Post(SpawnPedestrian), null, delay, delay);
    }

    private void SpawnPedestrian()
    {
        if (CrossingState == CrossingStateEnum.Active) return;

        var name = Names[_rng.Next(Names.Length)];
        var ped = new Pedestrian
        {
            Id = $"Pedestrian-{_pedIdCounter++}",
            Name = name,
            Direction = SpawnSides[_rng.Next(SpawnSides.Length)],
            WaitTime = 0,
            Status = PedestrianStatus.Waiting,
            EnteredAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Age = Ages[_rng.Next(Ages.Length)],
            Speed = Speeds[_rng.Next(Speeds.Length)]
        };

        if (Pedestrians.Count >= 12) Pedestrians.RemoveAt(0);
        Pedestrians.Add(ped);
        UpdatePedCounts();
    }

    private void UpdateWaitTimes()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        bool changed = false;
        foreach (var p in Pedestrians.Where(p => p.Status == PedestrianStatus.Waiting))
        {
            p.WaitTime = (int)((now - p.EnteredAt) / 1000);
            changed = true;
        }
        if (changed)
        {
            AvgWaitTime = Math.Max(1, Math.Round(AvgWaitTime + (_rng.NextDouble() - 0.5) * 0.8, 1));
        }
    }

    partial void OnPeakHourActiveChanged(bool value) => StartPedestrianSpawner();

    public void Dispose()
    {
        Pedestrians.CollectionChanged -= OnPedestriansCollectionChanged;
        _phaseTimer?.Dispose();
        _pedSpawnTimer?.Dispose();
        _waitUpdateTimer?.Dispose();
        _clockTimer?.Dispose();
        _peakTimer?.Dispose();
    }
}
