# Quickstart — gregPlugin.ExplorerBridge

> UnityExplorer + gregCore, butterweich.

Repo: [https://github.com/mleem97/gregPlugin.ExplorerBridge](https://github.com/mleem97/gregPlugin.ExplorerBridge) · Version: `0.1.0` · License: Apache-2.0.

## 1. Clone

```bash
git clone https://github.com/mleem97/gregPlugin.ExplorerBridge.git
cd gregPlugin.ExplorerBridge
```

## 2. Build

```bash
# Sync game/loader assemblies first (repo root helper)
../ModRepositories/tools/sync-melon-assemblies.sh

dotnet build gregPlugin.ExplorerBridge.csproj -c Release
```

The DLL lands in `bin/Release/net6.0/gregPlugin.ExplorerBridge.dll`.

## 3. Install

Copy the DLL to the game **Plugins** folder (not Mods!):

```bash
# Linux example
cp bin/Release/net6.0/gregPlugin.ExplorerBridge.dll \
  "$HOME/.local/share/Steam/steamapps/common/Data Center/Plugins/"
```

(Or from the repo root: `./build.sh ExplorerBridge --deploy`.)

## 4. Use

1. Requirements: UnityExplorer (MelonLoader IL2CPP build) in `Mods/`;
   gregCore recommended.
2. Start the game. The bridge links UnityExplorer lazily and logs
   `Tracking UnityExplorer menu`.
3. If UnityExplorer and Trainer shared `F7`, UnityExplorer now toggles on
   `Insert` (one-time automatic fix, logged + toasted).
4. Open UnityExplorer: camera/movement lock centrally, cursor shows.
   Close it: locks release. Toggle it from the F1 hub too.

Details: [README.md](README.md), [docs/USAGE.md](docs/USAGE.md).
If you run into problems: file an issue
([Issues](https://github.com/mleem97/gregPlugin.ExplorerBridge/issues)) or read
[CONTRIBUTING.md](CONTRIBUTING.md).
