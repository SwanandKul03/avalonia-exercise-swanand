using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaloniaExercise.Models;
using AvaloniaExercise.Models.Impl;

namespace AvaloniaExercise.ViewModels;

public enum CrossingVisualState
{
    Idle,
    Requested,
    Active,
    Cooldown
}

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly IPedestrianSensorService _pedestrianSensorService;
    private readonly ITrafficLightService _trafficLightService;
    private readonly Dictionary<Guid, EventHandler<PedestrianStatus>> _statusChangedHandlers = new();
    private readonly Timer _clockTimer;
    private readonly Timer _statsTimer;
    private DateTime? _phaseExpiryUtc;
    private bool _pendingRequestVisual;

    [ObservableProperty] private CrossingVisualState _crossingState = CrossingVisualState.Idle;
    [ObservableProperty] private int _countdown;
    [ObservableProperty] private int _totalCrossings;
    [ObservableProperty] private double _avgWaitTime;
    [ObservableProperty] private bool _peakHourActive;
    [ObservableProperty] private string _currentTime = "";
    [ObservableProperty] private int _queueSize;
    [ObservableProperty] private int _crossingCount;
    [ObservableProperty] private int _humanCount;
    [ObservableProperty] private int _animalCount;

    public ObservableCollection<Pedestrian> Pedestrians { get; } = new();
    public IEnumerable<Pedestrian> LeftSidePedestrians => Pedestrians.Where(IsOnLeftSide);
    public IEnumerable<Pedestrian> RightSidePedestrians => Pedestrians.Where(p => !IsOnLeftSide(p));

    public MainWindowViewModel(IPedestrianSensorService pedestrianSensorService, ITrafficLightService trafficLightService)
    {
        _pedestrianSensorService = pedestrianSensorService;
        _trafficLightService = trafficLightService;

        foreach (var pedestrian in _pedestrianSensorService.Pedestrians)
            AddPedestrian(pedestrian);

        _pedestrianSensorService.PedestriansChanged += OnPedestriansChanged;
        _trafficLightService.StatusChanged += OnTrafficLightStatusChanged;
        _trafficLightService.IsCrossingRequestedChanged += OnCrossingRequestedChanged;
        _trafficLightService.CrossingTimeExpiryUtcChanged += OnCrossingTimeExpiryChanged;

        UpdateClock();
        UpdateDerivedState();

        _clockTimer = new Timer(_ => Dispatcher.UIThread.Post(UpdateClock), null, 0, 1000);
        _statsTimer = new Timer(_ => Dispatcher.UIThread.Post(UpdateDerivedState), null, 0, 500);
    }

    private void UpdateClock() => CurrentTime = DateTime.Now.ToString("HH:mm:ss");

    private void UpdateDerivedState()
    {
        var waiting = Pedestrians.Where(p => p.Status == PedestrianStatus.WaitingToCross).ToList();
        var crossing = Pedestrians.Where(p => p.Status == PedestrianStatus.Crossing).ToList();
        bool hasPendingRequest = _trafficLightService.IsCrossingRequested || _pendingRequestVisual;
        double avgWait = waiting.Count == 0
            ? 0
            : Math.Round(waiting.Average(p => (DateTime.UtcNow - p.ArrivedAtUtc).TotalSeconds), 1);

        QueueSize = waiting.Count;
        CrossingCount = crossing.Count;
        HumanCount = Pedestrians.Count(p => p.Species == PedestrianSpecies.Human);
        AnimalCount = Pedestrians.Count - HumanCount;
        PeakHourActive = Pedestrians.Count >= 10 || QueueSize >= 6;
        AvgWaitTime = avgWait;

        Countdown = _phaseExpiryUtc is { } expiry
            ? Math.Max(0, (int)Math.Ceiling((expiry - DateTime.UtcNow).TotalSeconds))
            : 0;

        CrossingState = _trafficLightService.Status switch
        {
            TrafficLightStatus.Green when hasPendingRequest => CrossingVisualState.Requested,
            TrafficLightStatus.Green => CrossingVisualState.Idle,
            TrafficLightStatus.Red => CrossingVisualState.Active,
            TrafficLightStatus.Amber when hasPendingRequest => CrossingVisualState.Requested,
            TrafficLightStatus.Amber => CrossingVisualState.Cooldown,
            _ => CrossingVisualState.Idle
        };

        OnPropertyChanged(nameof(LeftSidePedestrians));
        OnPropertyChanged(nameof(RightSidePedestrians));
    }

    [RelayCommand]
    private async Task TriggerCrossingAsync()
    {
        if (_trafficLightService.IsCrossingRequested || _trafficLightService.Status != TrafficLightStatus.Green)
            return;

        _pendingRequestVisual = true;
        _phaseExpiryUtc = DateTime.UtcNow.AddSeconds(TrafficLightService.RequestedAmberTimeSeconds);
        UpdateDerivedState();

        await _trafficLightService.RequestCrossingAsync();
    }

    private void OnTrafficLightStatusChanged(object? sender, TrafficLightStatus e)
        => Dispatcher.UIThread.Post(() =>
        {
            switch (e)
            {
                case TrafficLightStatus.Green:
                    _phaseExpiryUtc = null;
                    _pendingRequestVisual = false;
                    break;
                case TrafficLightStatus.Red:
                    _phaseExpiryUtc ??= DateTime.UtcNow.AddSeconds(TrafficLightService.RedCrossingTimeSeconds);
                    _pendingRequestVisual = false;
                    break;
                case TrafficLightStatus.Amber when _trafficLightService.IsCrossingRequested || _pendingRequestVisual:
                    _phaseExpiryUtc = DateTime.UtcNow.AddSeconds(TrafficLightService.RequestedAmberTimeSeconds);
                    break;
                case TrafficLightStatus.Amber:
                    _phaseExpiryUtc = DateTime.UtcNow.AddSeconds(TrafficLightService.CooldownAmberTimeSeconds);
                    break;
            }

            UpdateDerivedState();
        });

    private void OnCrossingRequestedChanged(object? sender, bool e)
        => Dispatcher.UIThread.Post(() =>
        {
            if (e)
            {
                _pendingRequestVisual = true;
                _phaseExpiryUtc ??= DateTime.UtcNow.AddSeconds(TrafficLightService.RequestedAmberTimeSeconds);
            }
            else if (_trafficLightService.Status != TrafficLightStatus.Green)
            {
                _pendingRequestVisual = false;
            }

            UpdateDerivedState();
        });

    private void OnCrossingTimeExpiryChanged(object? sender, DateTime? e)
        => Dispatcher.UIThread.Post(() =>
        {
            _phaseExpiryUtc = e;
            UpdateDerivedState();
        });

    private void OnPedestriansChanged(object? sender, PedestriansChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            switch (e.Operation)
            {
                case PedestrianOperation.Arrived:
                    AddPedestrian(e.Pedestrian);
                    break;
                case PedestrianOperation.Left:
                    RemovePedestrian(e.Pedestrian);
                    if (e.Pedestrian.Status == PedestrianStatus.Crossed)
                        TotalCrossings++;
                    break;
            }

            UpdateDerivedState();
        });
    }

    private void AddPedestrian(Pedestrian pedestrian)
    {
        if (Pedestrians.Any(existing => existing.Id == pedestrian.Id))
            return;

        EventHandler<PedestrianStatus> handler = (_, _) => Dispatcher.UIThread.Post(UpdateDerivedState);
        pedestrian.StatusChanged += handler;
        _statusChangedHandlers[pedestrian.Id] = handler;
        Pedestrians.Add(pedestrian);
    }

    private void RemovePedestrian(Pedestrian pedestrian)
    {
        var existing = Pedestrians.FirstOrDefault(item => item.Id == pedestrian.Id);
        if (existing is null)
            return;

        if (_statusChangedHandlers.Remove(existing.Id, out var handler))
            existing.StatusChanged -= handler;

        Pedestrians.Remove(existing);
    }

    private static bool IsOnLeftSide(Pedestrian pedestrian)
        => (pedestrian.Id.GetHashCode() & 1) == 0;

    public void Dispose()
    {
        _pedestrianSensorService.PedestriansChanged -= OnPedestriansChanged;
        _trafficLightService.StatusChanged -= OnTrafficLightStatusChanged;
        _trafficLightService.IsCrossingRequestedChanged -= OnCrossingRequestedChanged;
        _trafficLightService.CrossingTimeExpiryUtcChanged -= OnCrossingTimeExpiryChanged;

        foreach (var pedestrian in Pedestrians.ToList())
        {
            if (_statusChangedHandlers.Remove(pedestrian.Id, out var handler))
                pedestrian.StatusChanged -= handler;
        }

        _clockTimer.Dispose();
        _statsTimer.Dispose();
    }
}
