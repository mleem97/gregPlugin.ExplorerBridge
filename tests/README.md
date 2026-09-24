# tests — gregPlugin.ExplorerBridge

Tests, fixtures, and test documentation.

Back: [README.md](../README.md) · Docs: [docs/INDEX.md](../docs/INDEX.md).

## What can be tested without the game

- `dotnet build gregPlugin.ExplorerBridge.csproj -c Release` must stay at
  **0 warnings, 0 errors**.

## In-game checklist (see docs/COMPATIBILITY.md)

1. Plugin loads before UnityExplorer; F7 collision pre-seeded (log line).
2. UE menu open → locks held + cursor; close → released.
3. F1 hub toggles UE; HUD shows live key.
4. Without gregCore / without UE: graceful idle, no errors.
