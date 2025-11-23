# Unity Supercross Prototype (Unity 6, Built-In 3D)

A small, fully code-driven Supercross-inspired motorcycle racing prototype for Unity 6 (6000.2.x) using the Built-In Render Pipeline and the classic Input system. Drop the provided `Assets/Scripts` folder into a fresh project, add `GameBootstrap` to an empty `GameRoot` object, and press Play to race a lap-based indoor dirt track against two AI riders.

## Features
- Procedurally constructed arena track with tabletop, whoops section, and bermed corners using primitives.
- Player motorcycle with arcade-friendly physics, boost, and lean visuals.
- Two AI racers following the same waypoint track with slight randomness.
- Lap checkpoints, live position tracking, lap timers, and race finish handling.
- Minimal HUD built at runtime (no prefabs) plus pre-race countdown gating controls.

## Quick Start
See [`UNITY_SETUP.md`](UNITY_SETUP.md) for step-by-step setup and controls on macOS with Unity Hub.
