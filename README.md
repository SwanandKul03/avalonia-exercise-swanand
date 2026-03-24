Real-Time Pedestrian Control System

A real-time monitoring dashboard for a pedestrian crossing system, built using Avalonia and MVVM.

Overview

This application simulates a traffic-controlled pedestrian crossing where users can monitor live system behavior and trigger crossing events.

It models a realistic signal workflow

Vehicles move during normal operation
A crossing request is triggered by the user
The system transitions through caution and stop phases
Pedestrians are allowed to cross
The system resets back to normal flow

The interface is designed as a control dashboard, balancing visual clarity with real-time feedback.

Key Features

Real-time signal state transitions (Idle, Requested, Active, Cooldown)
Live countdown integrated into traffic lights
Dynamic pedestrian queue with simulated arrivals
Interactive crossing trigger control
Peak-hour simulation and live system clock
Visual crossing scene with roads, signals, and pedestrian distribution
Status panel for tracking pedestrian activity

Design Approach

The UI is designed to reflect a real-world traffic control panel

Traffic-light color semantics (red, amber, green) drive system understanding
A center-weighted layout represents the physical crossing environment
Metrics and system state are separated for clarity
Motion and visual feedback highlight active system states

The goal was to create a system that is both **functionally clear** and **visually intuitive**.

Tech Stack

1) .NET 8
2) Avalonia UI
3) CommunityToolkit.Mvvm
4) FontAwesome Icons

Architecture

The project follows a clean MVVM structure

Models - Define pedestrian data and signal states
ViewModels - Handle simulation logic, timers, and state transitions
Views - Render the dashboard UI using XAML
Converters - Map system state to UI behavior (colors, labels, visibility)

Running the Project

dotnet run --project "Real-Time Pedestrian Control System/Real-Time Pedestrian Control System.csproj"

