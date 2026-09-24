# Source layout

All C# source lives in `src/`, current game/loader assemblies in `references/`
(symlinks into the local Data Center install — never commit DLLs), and project
documentation in `docs/`.

| File | Why it exists |
|---|---|
| `src/ExplorerBridgePlugin.cs` | MelonPlugin entry (pre-seed, lazy link, poll, re-verify) |
| `src/MyPluginInfo.cs` | Plugin id/name/version constants |
| `src/GregHost.cs` | gregCore soft-dependency probe |
| `src/BridgeConfig.cs` | Prefs + F1-effective values + pref-file reader |
| `src/UnityExplorerLink.cs` | Reflection-only UE link (ShowMenu get/set) |
| `src/CoreLink.cs` | Only file referencing gregCore (JIT split) |
