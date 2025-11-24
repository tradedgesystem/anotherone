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

## Trading Journal Google Sheet
Use the Apps Script in `scripts/trading_journal.gs` to generate a Google Sheet called **Trading Journal** with structured trade logging and analytics.

### How to use
1. Open [Google Apps Script](https://script.google.com/) and create a new project.
2. Copy the contents of `scripts/trading_journal.gs` into the editor.
3. Save and run the `createTradingJournal()` function. Grant the requested Drive and Sheets permissions.
4. A new spreadsheet named **Trading Journal** will be created with:
   - A **Trades** sheet containing only dropdown inputs (date picker, Trade 1–5, Strategy Used, Liquidity Level) so no manual typing is required.
   - Data-validation lists restricting Strategy Used and Liquidity Level to the exact options provided.
   - Built-in room for up to five trades per day across many days.
   - An **Analytics** sheet with a live pie chart showing the percentage distribution of strategies across all logged trades.
