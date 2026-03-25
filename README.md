# Real-Time Pedestrian Control System

A real-time desktop dashboard built with Avalonia that simulates and monitors a smart pedestrian crossing system.

The application presents traffic signal states, pedestrian flow, crossing activity, and operator controls within a single, responsive interface designed for clarity and quick decision-making.


## Objective

To design a monitoring interface that:

- Clearly communicates real-time system state  
- Reflects real-world traffic behavior  
- Supports quick operator interaction  
- Balances visual richness with usability  


## Project Overview

This project extends the provided exercise scaffold into a structured control dashboard.

It simulates a live crossing environment where pedestrians are generated dynamically, signals transition through timed phases, and the UI continuously reflects system changes using MVVM-driven updates.

The interface is designed to resemble a compact control panel used for monitoring and managing pedestrian crossings.


## Core Features

- Real-time traffic signal lifecycle (Green → Amber → Red → Cooldown)  
- Countdown timers integrated into signal states  
- Dynamic entity generation (pedestrians, dogs, cats)  
- Live queue tracking and crossing activity  
- Operator-triggered crossing control  
- Peak-hour simulation based on crowd conditions  
- Average wait-time calculation  
- Visual crossing scene with zebra crossing and vehicle indicators  
- Reactive UI powered by MVVM bindings  


## Simulation Flow

The system follows a structured crossing cycle:

1. Vehicles proceed under a green signal  
2. Operator triggers a crossing request  
3. Signal transitions to amber (prepare phase)  
4. Vehicles stop (red signal)  
5. Pedestrians begin crossing  
6. System enters cooldown phase  
7. Returns to normal traffic flow  


### Signal Timings

- Request phase (Amber): 5s  
- Active crossing (Red): 15s  
- Cooldown phase (Amber): 5s  


## Design Decisions

### **Real-World Alignment**
The UI mirrors a physical crossing system:
- Vertical signal lights  
- Zebra crossing layout  
- Vehicle and pedestrian separation  

→ Reduces learning curve and improves immediate understanding  


### **Semantic Color System**
- Green → Proceed  
- Amber → Caution / Transition  
- Red → Stop  

→ Colors directly represent system state, not decoration  


### **Clear Information Hierarchy**
- Left → Signal + Control  
- Center → Crossing visualization  
- Right → Entity list  
- Bottom → Metrics  

→ Enables fast scanning and situational awareness  


### **State-Driven UI**
- Entire interface is controlled by a single source: signal state  
- All UI elements react through bindings and converters  

→ Ensures consistency and predictable behavior  


### **Minimal Cognitive Load**
- Uses short, familiar labels (`PROCEED`, `CAUTION`, `STOP`)  
- Visual cues reduce need for reading  

→ Improves usability in time-sensitive scenarios  


### **Purposeful Motion**
- Active signals pulse subtly  
- Countdown is embedded in signal lights  

→ Motion highlights critical changes without distraction  


## Architecture

- MVVM (Model-View-ViewModel)  
- Observable properties for real-time updates  
- Timer-driven simulation logic  
- Converter-based UI state translation  


## Tech Stack

- .NET 10  
- Avalonia UI 11  
- CommunityToolkit.Mvvm  
- Font Awesome Icons


## Design & Development Methodology

The dashboard was developed through an iterative, user-centered process.

- **Paper Prototyping**  
  Initial layouts and interaction flows were explored using low-fidelity sketches to quickly define information hierarchy and system structure.

- **Figma Design Prototype**  
  A high-fidelity prototype was created using a component-driven design system. Reusable components, variants, and shared styles ensured scalability, consistency, and a clear visual hierarchy. Semantic traffic colors (red, amber, green) were used to align with real-world expectations.

- **Development (Avalonia + MVVM)**  
  The validated design was implemented using Avalonia with MVVM architecture, ensuring separation of concerns between UI, logic, and data. State-driven updates, bindings, and reusable components were used to maintain consistency between design and implementation.
