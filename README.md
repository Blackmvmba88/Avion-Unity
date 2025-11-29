# Avion-Unity

Flight simulator built in Unity with advanced aerodynamics, modular aircraft systems, dynamic environment, and full multiplatform support. Unity edition of the Avion project.

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Core game systems (GameManager)
│   ├── Physics/        # Flight physics calculations (FlightPhysics)
│   ├── Aircraft/       # Aircraft control systems (AircraftController)
│   ├── Environment/    # Weather and atmosphere (EnvironmentManager)
│   ├── Controls/       # Input handling (InputManager)
│   ├── UI/             # HUD and interface components (HUD)
│   └── Network/        # Multiplayer functionality (NetworkManager)
├── Prefabs/            # Reusable game object prefabs
├── Materials/          # Material assets
├── Shaders/            # Custom shader code
├── Scenes/             # Unity scenes (Main.unity)
├── Audio/              # Sound effects and music
├── FX/                 # Visual effects and particles
└── Localization/       # Language translation files
```

## Features

### Flight Physics
- Realistic aerodynamic forces (lift, drag, thrust)
- International Standard Atmosphere model
- Configurable aircraft properties

### Aircraft Systems
- Full control surface simulation (pitch, roll, yaw)
- Throttle and engine management
- Landing gear and flaps (placeholder)

### Environment
- Dynamic day/night cycle
- Weather system with multiple conditions
- Wind and turbulence simulation
- Altitude-based atmospheric calculations

### Controls
- Keyboard, mouse, and gamepad support
- Configurable input sensitivity
- Multiple control schemes

### HUD Display
- Speed and altitude indicators
- Attitude and heading display
- Engine status and warnings
- G-force meter

### Networking
- Multiplayer session management
- Player synchronization
- Chat functionality (placeholder)

## Controls

| Action | Keyboard | Gamepad |
|--------|----------|---------|
| Pitch | W/S or Up/Down | Left Stick Y |
| Roll | A/D or Left/Right | Left Stick X |
| Yaw | Q/E | Right Stick X |
| Throttle Up | Left Shift | RT |
| Throttle Down | Left Ctrl | LT |
| Brakes | B | A |
| Landing Gear | G | X |
| Flaps | F | Y |
| Pause | ESC/P | Start |

## Getting Started

1. Open the project in Unity 2022.3 LTS or newer
2. Open the `Assets/Scenes/Main.unity` scene
3. Press Play to start the simulator

## Requirements

- Unity 2022.3 LTS or newer
- .NET Standard 2.1

## License

See LICENSE file for details.
