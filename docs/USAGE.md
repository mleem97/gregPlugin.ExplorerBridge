# Usage — gregPlugin.ExplorerBridge

## What the bridge does automatically

1. **Toggle fix:** if UnityExplorer and gregMod.Trainer share `F7`,
   UnityExplorer is moved to `Insert` before it initializes (logged; toasted
   once both mods are up if anything still collides).
2. **Input sync:** UnityExplorer menu open → camera/movement/interact locked
   centrally + cursor visible. Closed → released.
3. **Hub:** toggle UnityExplorer from the F1 hub; HUD shows its live key.

## Recommended UnityExplorer settings (`MelonPreferences.cfg`, `[UnityExplorer]`)

| Setting | Recommendation | Why |
|---|---|---|
| `Startup Delay Time` | `3.0`–`5.0` (default `1.0`) | Lets gregCore + mods finish booting before UE builds UI |
| `Force Unlock Mouse` | keep `true` | Matches the bridge-held cursor; no fight |
| `Disable EventSystem override` | keep `false` | Works with the bridge; set `true` + restart only if UI clicks misbehave |
| `Hide On Startup` | your choice | Bridge syncs either way once linked |
| `UnityExplorer Toggle` | managed by bridge on collision | Change freely if no collision; bridge re-checks |

## Coexistence rules

- **Freecam:** UnityExplorer's freecam and gregMod.NoClip both drive the
  camera — use one at a time. The bridge does not touch freecam toggles.
- **C# console / hooks:** UnityExplorer's console and hook manager bypass
  gregCore (by design — it's a debugger). Locks still apply while its menu
  is open.
- **Mod hotkeys keep working** while the explorer menu is open (direct
  keyboard polling is unaffected by input locks).
- **Without gregCore:** the toggle guard still works; lock sync is skipped
  (logged once).
- **Without UnityExplorer:** the bridge idles with one log line. Safe to
  keep installed.

## Settings reference (bridge)

| Setting | F1 (gregCore) | Prefs | Default | Meaning |
|---|---|---|---|---|
| Bridge enabled | yes | yes | ON | Master switch |
| Sync input locks | yes | yes | ON | Hold locks while UE menu open |
| Auto-fix toggle collision | — | yes | ON | Pre-seed UE fallback key on collision |
| UE fallback toggle key | — | yes | `Insert` | Written into UE prefs when fixing |

Prefs live in `MelonPreferences.cfg`, category `gregPlugin.ExplorerBridge`.
