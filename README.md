# gregPlugin.ExplorerBridge

> UnityExplorer + gregCore, butterweich: no shared hotkeys, no input fights.

Copy `gregPlugin.ExplorerBridge.dll` to `Data Center/Plugins/`.
Requires [UnityExplorer](https://github.com/Ziggista/UnityExplorer) (MelonLoader
IL2CPP build) and, for the full experience,
[gregCore](https://github.com/mleem97/gregCore). Works without gregCore too
(pre-seed guard stays active, lock sync is skipped).

![License](https://img.shields.io/github/license/mleem97/gregPlugin.ExplorerBridge?style=for-the-badge)

## Links

- **Repository:** [https://github.com/mleem97/gregPlugin.ExplorerBridge](https://github.com/mleem97/gregPlugin.ExplorerBridge)
- **Issues:** [https://github.com/mleem97/gregPlugin.ExplorerBridge/issues](https://github.com/mleem97/gregPlugin.ExplorerBridge/issues)
- **Releases:** [https://github.com/mleem97/gregPlugin.ExplorerBridge/releases](https://github.com/mleem97/gregPlugin.ExplorerBridge/releases)

## Overview

UnityExplorer is the standard in-game inspector — but next to gregCore it
rubs in three places:

1. **Shared hotkey:** UnityExplorer (default `F7`) and gregMod.Trainer
   (default `F7`) toggle at the same keypress.
2. **Input fight:** UnityExplorer's open menu vs. gregCore's camera/movement
   locks and cursor control (camera spins while you click explorer panels).
3. **No hub presence:** UnityExplorer can't be toggled from the gregCore F1 hub.

**gregPlugin.ExplorerBridge** (a MelonLoader plugin, so it loads before mods)
fixes all three:

1. **Toggle-collision guard:** detects the shared key from the prefs file and
   pre-seeds UnityExplorer's toggle with a free fallback (`Insert`) before it
   initializes — one press, one UI.
2. **Input-lock sync:** while UnityExplorer's menu is open, gregCore holds
   camera/movement/interact locks + cursor; released on close.
3. **F1-hub integration:** open/close UnityExplorer from the hub, HUD hint
   with the live toggle key, F1 config entries.

UnityExplorer is linked by **reflection only** (it ships separately via the
Workshop): missing or renamed APIs degrade to idle logging, never errors.

See [docs/INDEX.md](docs/INDEX.md) for the complete documentation, and
[docs/USAGE.md](docs/USAGE.md) for recommended UnityExplorer settings.

## Compatibility

| Platform    | Status    |
| ----------- | --------- |
| Windows x64 | Supported |
| Linux x64   | Supported |

UnityExplorer: MelonLoader IL2CPP CoreCLR build (0.6+ line, incl. the
Ziggista/yukieiji fork). Game/loader baseline: see
[docs/COMPATIBILITY.md](docs/COMPATIBILITY.md).

## Installation

See [QUICKSTART.md](QUICKSTART.md).

## Build from Source

```bash
git clone https://github.com/mleem97/gregPlugin.ExplorerBridge.git
cd gregPlugin.ExplorerBridge
dotnet build gregPlugin.ExplorerBridge.csproj -c Release
```

Details: [QUICKSTART.md](QUICKSTART.md), [CONTRIBUTING.md](CONTRIBUTING.md).

## Repository Layout

```
├── README.md            # This file
├── QUICKSTART.md        # Quickstart
├── CHANGELOG.md         # Changelog (Keep a Changelog)
├── CONTRIBUTING.md      # Contributing
├── SECURITY.md          # Security reports
├── CODE_OF_CONDUCT.md   # Code of conduct
├── AGENTS.md            # Notes for AI agents
├── LICENSE              # Apache-2.0
├── VERSION              # Single source of truth for the version
├── manifest.json        # Plugin manifest
├── docs/                # Documentation ([Index](docs/INDEX.md))
├── scripts/             # Build/helper scripts
├── tests/               # Tests
├── references/          # Game/loader assemblies (symlinks, never committed)
├── examples/            # Examples
└── src/                 # C# source
```

## API Documentation

See [`docs/INDEX.md`](docs/INDEX.md).

## Credits

| Role       | Contributor                                        |
| ---------- | -------------------------------------------------- |
| **Codebase** | [mleem97](https://github.com/mleem97)            |
| UnityExplorer | [sinai-dev](https://github.com/sinai-dev/UnityExplorer), [yukieiji](https://github.com/yukieiji/UnityExplorer), [Ziggista](https://github.com/Ziggista/UnityExplorer) |

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

Apache-2.0 — see [`LICENSE`](LICENSE). UnityExplorer itself is third-party
(see its repository for its license).

## 🚀 Join the gregFramework Team!

Do you enjoy building mods, tools, or docs? Get in touch: **apply@gregframework.eu** or via
[Discord](https://discord.gg/greg) — Code, Assets, Docs, Testing, Infra, Community.

---

**gregFramework — powered by the community.**
