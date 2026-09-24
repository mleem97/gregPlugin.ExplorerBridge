# examples — gregPlugin.ExplorerBridge

Examples and further material.

Back: [README.md](../README.md) · Docs: [docs/INDEX.md](../docs/INDEX.md).

## Smooth-coexistence recipe

1. Install UnityExplorer (MelonLoader IL2CPP build) in `Mods/` and this
   bridge in `Plugins/`, gregCore in `Mods/`.
2. In `[UnityExplorer]` prefs, raise `Startup Delay Time` to `3.0`.
3. Start the game: bridge moves UnityExplorer's toggle off `F7` (Trainer
   keeps it), links the menu, and from then on opening UnityExplorer locks
   game input centrally — click explorer panels without camera spin.
4. Toggle UnityExplorer from the F1 hub (`explorerbridge` entry) or its own
   key. Use either UnityExplorer's freecam or gregMod.NoClip — not both.
