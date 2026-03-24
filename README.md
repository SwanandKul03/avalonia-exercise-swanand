# Real-Time Pedestrian Control System

![Platform](https://img.shields.io/badge/platform-.NET%208-blue)
![UI](https://img.shields.io/badge/UI-Avalonia-0f172a)
![Pattern](https://img.shields.io/badge/architecture-MVVM-15803d)
![Status](https://img.shields.io/badge/status-ready%20for%20GitHub-success)

A desktop dashboard simulator for monitoring and controlling a pedestrian crossing system.

This project recreates a live roadside control panel with a timed signal cycle, pedestrian queue simulation, crossing-state transitions, operator controls, and a visually structured traffic scene. It is built with Avalonia UI on .NET 8 and follows an MVVM architecture.

## Why This Project

The goal of this project is to demonstrate how a pedestrian control system can be represented as a modern desktop dashboard.

It combines:

- real-time state simulation
- dashboard-style monitoring
- traffic signal logic
- pedestrian queue behavior
- a polished control-room-inspired interface

## Key Features

- Real-time traffic signal workflow with idle, requested, active, and cooldown phases
- Pedestrian spawning and queue management
- Live countdown display inside the signal lights
- Peak-hour simulation and live clock
- Center crossing-zone visualization with sidewalks, vehicles, zebra crossing, and signals
- Pedestrian tracking list with live status updates
- Reusable styling and converter-driven UI behavior
- Clean MVVM separation between logic and presentation

## Demo Flow

The application models a simple crossing lifecycle:

1. Vehicles move during the idle phase.
2. The operator triggers a crossing request.
3. The signal transitions into caution.
4. The crossing becomes active and pedestrians are allowed to move.
5. The system enters cooldown and returns to normal traffic flow.

## Tech Stack

- .NET 8
- Avalonia 11
- CommunityToolkit.Mvvm
- Font Awesome icons via Projektanker.Icons.Avalonia.FontAwesome

## Architecture

### Startup Layer

- [Real-Time Pedestrian Control System/Program.cs](Real-Time%20Pedestrian%20Control%20System/Program.cs)
    Initializes Avalonia and registers the Font Awesome icon provider.
- [Real-Time Pedestrian Control System/App.axaml](Real-Time%20Pedestrian%20Control%20System/App.axaml)
    Loads the Fluent theme and shared custom styles.
- [Real-Time Pedestrian Control System/App.axaml.cs](Real-Time%20Pedestrian%20Control%20System/App.axaml.cs)
    Creates the main window and assigns the view model.

### Domain Layer

- [Real-Time Pedestrian Control System/Models/CrossingState.cs](Real-Time%20Pedestrian%20Control%20System/Models/CrossingState.cs)
    Defines the signal-state lifecycle.
- [Real-Time Pedestrian Control System/Models/Pedestrian.cs](Real-Time%20Pedestrian%20Control%20System/Models/Pedestrian.cs)
    Stores pedestrian attributes such as ID, side, wait time, and status.

### Logic Layer

- [Real-Time Pedestrian Control System/ViewModels/MainViewModel.cs](Real-Time%20Pedestrian%20Control%20System/ViewModels/MainViewModel.cs)
    Controls timers, countdowns, pedestrian spawning, queue calculations, state transitions, and the trigger command.

### Presentation Layer

- [Real-Time Pedestrian Control System/Views/MainWindow.axaml](Real-Time%20Pedestrian%20Control%20System/Views/MainWindow.axaml)
    Defines the complete dashboard UI.
- [Real-Time Pedestrian Control System/Views/MainWindow.axaml.cs](Real-Time%20Pedestrian%20Control%20System/Views/MainWindow.axaml.cs)
    Minimal code-behind for the main window shell.

### Visual Translation Layer

- [Real-Time Pedestrian Control System/Converters/Converters.cs](Real-Time%20Pedestrian%20Control%20System/Converters/Converters.cs)
    Converts system state into labels, colors, visibility rules, and signal behavior.
- [Real-Time Pedestrian Control System/Styles/Theme.axaml](Real-Time%20Pedestrian%20Control%20System/Styles/Theme.axaml)
    Centralizes typography, panel surfaces, metric styling, and reusable UI primitives.

## UI Design Factors

The final interface is shaped by a few consistent design principles:

- Dark control-room palette for a dashboard feel
- Green, amber, and red as semantic traffic colors
- Large metrics for operational readability
- Center-weighted crossing scene to mimic a real roadway overview
- Minimal but expressive iconography instead of emoji-based symbols
- Reusable glass-card surfaces for consistent panel styling
- Converter-driven state styling so visuals stay synchronized with logic

## Project Structure

```text
Real-Time Pedestrian Monitoring/
├── README.md
├── .gitignore
└── Real-Time Pedestrian Control System/
        ├── Real-Time Pedestrian Control System.sln
        ├── Real-Time Pedestrian Control System.csproj
        ├── Program.cs
        ├── App.axaml
        ├── App.axaml.cs
        ├── Models/
        ├── ViewModels/
        ├── Views/
        ├── Converters/
        └── Styles/
```

## Build

Run from the workspace root:

```bash
dotnet build "Real-Time Pedestrian Control System/Real-Time Pedestrian Control System.sln"
```

## Run

Run from the workspace root:

```bash
dotnet run --project "Real-Time Pedestrian Control System/Real-Time Pedestrian Control System.csproj"
```

## Screenshots

Add screenshots here after publishing.

Recommended sections:

- Main dashboard overview
- Active crossing phase
- Requested or caution phase

## Repository Notes

- Generated build output is ignored through [/.gitignore](.gitignore)
- The application is currently a local simulation and does not connect to external hardware or live traffic sensors
- The root folder structure is intentionally kept clean so the local project tree matches the published GitHub structure

## Future Improvements

- Hardware or sensor integration
- Event logging and persistence
- Historical analytics charts
- Configuration for signal timing parameters
- Automated test coverage for the state machine and converters

## License

Add your preferred license before making the repository public for reuse.