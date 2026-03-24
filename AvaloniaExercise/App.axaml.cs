using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using System.Linq;
using AvaloniaExercise.Models;
using AvaloniaExercise.Models.Impl;
using AvaloniaExercise.ViewModels;
using AvaloniaExercise.Views;

namespace AvaloniaExercise;

public partial class App : Application
{
    private IPedestrianSensorService? _pedestrianSensorService;
    private ITrafficLightService? _trafficLightService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var pedestrianSensorService = new PedestrianSensorService();
        _pedestrianSensorService = pedestrianSensorService;
        _trafficLightService = new TrafficLightService(pedestrianSensorService);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(_pedestrianSensorService!, _trafficLightService!),
            };

            desktop.Exit += OnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        try
        {
            if (_pedestrianSensorService is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine("Exception whilst disposing: " + exception);
        }
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}
