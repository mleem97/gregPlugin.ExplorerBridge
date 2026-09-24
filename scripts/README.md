# scripts — gregPlugin.ExplorerBridge

Build/helper scripts.

Back: [README.md](../README.md) · Docs: [docs/INDEX.md](../docs/INDEX.md).

Builds run from the repository root with the shared helper:

```bash
# from ModRepositories/
./build.sh ExplorerBridge            # Release build
./build.sh ExplorerBridge --deploy   # build + copy DLL to Data Center/Plugins
```

`tools/sync-melon-assemblies.sh` keeps `references/*.dll` pointed at the live
game assemblies before building.
