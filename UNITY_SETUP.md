# Unity Setup (macOS, Unity Hub)

Follow these steps exactly to get the Supercross prototype running in Unity 6 (6000.2.x) using the Built-In Render Pipeline and the classic Input system.

1. **Open Unity Hub.**
2. **Create a new project.**
   - Editor version: **Unity 6 (6000.2.x)**.
   - Template: **3D (Built-In Render Pipeline)**.
   - Name it anything you like and choose a folder location.
3. **Quit Unity after the project is created.** Closing Unity prevents file locking while copying assets.
4. **Copy the provided scripts into the project.**
   - On macOS Finder, open the downloaded package folder for this prototype.
   - Copy the entire `Assets/Scripts` folder from this package.
   - Paste it into your new Unity project’s `Assets` folder (replace/merge if the folder already exists). The final path should be `YourProject/Assets/Scripts/...`.
5. **Reopen the project in Unity Hub.** Let Unity compile; the Console should show no errors once scripts finish compiling.
6. **Set up the scene (SampleScene is fine).**
   - Optionally delete any default objects except **Main Camera** and **Directional Light** (they are harmless to leave).
   - Create an empty GameObject and rename it **GameRoot**.
   - With GameRoot selected, click **Add Component** and choose `GameBootstrap`.
7. **Press Play.** Unity will build the track, spawn the player and two AI riders, run the countdown, and start the race.
8. **Controls.**
   - **W / Up Arrow:** Throttle forward
   - **S / Down Arrow:** Reverse / slow
   - **A / Left Arrow:** Steer left
   - **D / Right Arrow:** Steer right
   - **Space:** Brake
   - **Left Shift:** Boost (short burst with cooldown)
   - **R:** Restart race
   - **Esc:** Quit play mode (or exit build)
9. **Tweaking difficulty and feel.**
   - Select **GameRoot** while in Play Mode to adjust script parameters live.
   - `TrackBuilder` exposes **Total Laps** and **Track Width**; increasing width makes turns easier.
   - `MotorcycleController` parameters (acceleration, max speed, turn speed) live on the player object at runtime; tweak to taste.
   - `AIWaypointFollower` and `AIRacerController` expose speeds/turn responsiveness; lower speeds make the AI easier.

## Troubleshooting
- If the Console shows red errors, double-click them to locate the script and line. The most common causes are files not placed under `Assets/Scripts` or class names not matching filenames—ensure the entire folder was copied intact.
- Make sure the project uses the **classic Input system** (default for Built-In projects). If prompted to enable the new Input System, choose **No**.
- If play mode shows a blank scene, confirm `GameBootstrap` is attached to **GameRoot** and there are no missing scripts.
