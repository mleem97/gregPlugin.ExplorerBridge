# Compatibility — gregPlugin.ExplorerBridge

## Baseline (built and signature-checked against)

- Game: **Data Center** by **Waseku**.
- Loader: **MelonLoader 0.7.x**, plugin target **net6.0-x64**.
- UnityExplorer: MelonLoader IL2CPP CoreCLR build (Ziggista/yukieiji fork
  line), local DLL `UnityExplorer.ML.IL2CPP.CoreCLR.dll` (704 types).
- `gregCore` (optional): soft dependency — pre-seed works without it.

## Reverse-engineering evidence

- `UnityExplorer.UI.UIManager.ShowMenu`: static `bool`, **getter + setter**
  present → state sync both directions (poll + F1-hub toggle).
- `UIManager.UIRoot/UICanvas` exist (fallback detection path, currently unused).
- MelonPreferences API used: `CreateCategory`, `HasEntry`, `GetEntry`,
  generic `GetEntry<T>` (`.Value` set), `Save` — all verified against
  MelonLoader 0.7.3 dummies at compile time.
- Toggle identifiers observed live in `MelonPreferences.cfg`:
  `[UnityExplorer] "UnityExplorer Toggle" = "F7"`,
  `[gregMod.Trainer] "ToggleKey" = "F7"` → the F7 collision is real.
- F-key map (all taken, hence `Insert` fallback): F3 Potato, F4 FiberTrunk,
  F5 NoEOL, F6 Backplanes, F7 Trainer+UE, F8 MultiCable, F9 MusicPlayer,
  F10 NotesHUD, F11 MemeRoulette/Economics, F12 GameExport.

## Known limits (v1)

1. **In-game verification pending** — build passes (`0 warnings, 0 errors`);
   pre-seed adoption, lock sync feel, and hub toggle need a live game.
2. **Pre-seed assumption:** MelonLoader's `CreateEntry` returns the existing
   entry on duplicate identifiers (so UE adopts the pre-seeded value).
   Guarded by `HasEntry` + catch; if UE ever throws on duplicates, the bridge
   logs and stands down — but verify after MelonLoader updates.
3. **UE key caching unknown:** if UnityExplorer caches its toggle at init,
   a *live* rewrite needs one restart; the pre-seed path (pre-init) needs none.
4. **Freecam untouched by design** — UE freecam vs NoClip: use one at a time.
5. Game updates renaming `UIManager.ShowMenu` degrade to idle logging.

## Test status

- [x] `dotnet build -c Release` — clean (0 warnings, 0 errors).
- [ ] Plugin loads before UE; F7 collision pre-seeded to Insert (log line).
- [ ] UE menu open → camera/movement locked, cursor usable; close → released.
- [ ] F1 hub toggles UE; HUD shows live key.
- [ ] Without gregCore: pre-seed works, sync skipped with log.
- [ ] Without UnityExplorer: one idle line, no errors.
