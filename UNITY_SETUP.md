# Unity Dirt Track Prototype Setup (Unity 2022 LTS)

This repository contains the scripts needed to assemble a minimal 3D Supercross-style loop track with a player bike and AI opponent.

## Folder placement
Copy the files directly into your Unity project's `Assets/` folder keeping the same sub-folder structure:

- `Assets/Scripts/Player/MotorcycleController.cs`
- `Assets/Scripts/Player/PlayerInputController.cs`
- `Assets/Scripts/AI/AIWaypointFollower.cs`
- `Assets/Scripts/Track/Checkpoint.cs`
- `Assets/Scripts/Track/LapManager.cs`
- `Assets/Scripts/Camera/FollowCamera.cs`

## Scene assembly steps
1. **Scene basics**
   - Create a new 3D scene. Add an HDRI skybox from Unity's defaults (Window → Rendering → Lighting → Environment). Place a Directional Light angled to cast across the track.
   - Make a looped dirt track using Unity Terrain or low-poly mesh strips. Add box colliders along the sides (or simple walls) to keep riders on-course.
   - Place 5–10 empty GameObjects along the racing line as checkpoints; add a Box Collider (IsTrigger) and the `Checkpoint` script to each. Number them sequentially through `OrderIndex`.

2. **Player motorcycle prefab**
   - Create an empty GameObject `PlayerBike` and add a **Rigidbody** (mass ~180, drag 0.3, angular drag 0.6) and **Capsule Collider** sized around the body.
   - Build the bike visuals from primitives: scaled cubes for body/frame, thin cylinders for wheels. Parent the wheel meshes to empty child objects named `FrontWheel` and `RearWheel` positioned at axle height.
   - Add `MotorcycleController` and `PlayerInputController` to `PlayerBike`.
     - Assign `FrontWheel` and `RearWheel` to the script.
     - Optionally assign a child `LeanTransform` (e.g., the body mesh) so it tilts visually.
   - Set `LastCheckpointPosition/Rotation` by placing the bike on the start/finish area.

3. **Camera**
   - Create a Camera and add `FollowCamera`. Drag the player `Transform` into **Target**. Adjust **Offset** (e.g., `y=3`, `z=-6`) for a chase view.

4. **Lap and checkpoint system**
   - Create an empty GameObject `RaceManager` with a Box Collider (IsTrigger) covering the start/finish line.
   - Add `LapManager` to `RaceManager` and assign:
     - `Current Lap`, `Last Lap`, and `Best Lap` UI Text elements (create a simple Canvas with three Texts anchored top-left).
     - Drag the player bike into **Player Controller**.
     - Populate the **Checkpoints** list with your sequential checkpoint objects (5–10). The first checkpoint should be right after the start line.

5. **AI opponent**
   - Duplicate the player bike to create `AIBike`. Remove `PlayerInputController` and add `AIWaypointFollower`.
   - Create a set of empty GameObjects as waypoints along the course (ideally matching or near checkpoints). Assign them to the AI script in order.
   - Ensure `AIBike` shares the same Rigidbody and collider setup so it uses identical physics.

6. **Reset and boundaries**
   - For each checkpoint, position it slightly above ground so the player triggers it. The player can press **R** to reset to the last passed checkpoint location.
   - Add low walls, invisible box colliders, or Terrain collider edges around the track perimeter to prevent leaving the course.

7. **Testing controls**
   - Play mode controls: **W/Up Arrow** accelerate, **A/D or Left/Right** to lean/steer, **Space** to brake, **R** to reset.
   - AI should lap the course by following the provided waypoints.

You now have a lightweight dirt-track racing prototype ready for further iteration.
