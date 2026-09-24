# Architecture — gregPlugin.ExplorerBridge

> A MelonLoader plugin (loads before mods): pre-seed prefs, then mirror menu state into locks.

## Components

```text
boot ──► pref-file scan ──► pre-seed UE toggle ──► lazy UE link ──► poll ShowMenu ──► SetOpen(locks)
   │         (F7 vs F7?)       (before UE init)      (retry/300f)      (on change)      (gregCore, JIT-split)
   └─► F1 entries + hub (opener=toggler, closer) + HUD hint
```

| File | Responsibility |
|---|---|
| `src/ExplorerBridgePlugin.cs` | MelonPlugin entry: prefs, pre-seed, lazy link, poll loop, re-verify, unload release |
| `src/BridgeConfig.cs` | MelonPreferences + F1-effective values; raw pref-file reader (pre-init collision detect) |
| `src/UnityExplorerLink.cs` | Reflection-only UE link (`UIManager.ShowMenu` get/set); idle-on-missing |
| `src/CoreLink.cs` | **Only** file with gregCore references (JIT split): menu+locks, HUD, F1, hub, toasts |
| `src/GregHost.cs` | Soft-dependency probe |
| `src/MyPluginInfo.cs` | Plugin id/name/version constants |

## Data flows

1. **Pre-seed:** `OnInitializeMelon` reads `MelonPreferences.cfg` (`[UnityExplorer]`
   toggle vs `[gregMod.Trainer]` toggle, defaults `F7`). On collision (and
   `AutoFixToggleCollision`), writes the fallback key into UE's not-yet-created
   preference entry (or live entry), saves prefs.
2. **Link:** `OnUpdate` scans loaded assemblies for `UnityExplorer*` every 300
   frames until `UIManager.ShowMenu` resolves.
3. **Sync:** `ShowMenu` polled per frame; on change → `GregMenuRegistry.SetOpen
   ("unityexplorer", open)` (menu pre-registered with camera/movement/interact
   locks + cursor). Released on disable/unload too.
4. **Hub:** F1 opener toggles UE via the setter; closer forces closed; HUD key
   text reads the live UE toggle.
5. **Re-verify:** ~15 s after boot, live entries re-compared; lingering
   collision → warning + toast with manual steps.

## Failure handling

- No UE installed / renamed API → one idle log line, zero polling cost after give-up? (retry persists cheaply every 300 frames).
- Pref writes / reflection / core calls: individually try/caught; the bridge
  can never break UE init or game boot.
- Duplicate-entry semantics of MelonLoader are assumed (create-returns-existing);
  guarded by `HasEntry` checks + catch fallbacks.

Record changes here + [`CHANGELOG.md`](../CHANGELOG.md) (Unreleased).
