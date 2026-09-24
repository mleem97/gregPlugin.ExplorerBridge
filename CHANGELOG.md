# Changelog — gregPlugin.ExplorerBridge

Format: [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). Version: see [`VERSION`](VERSION).

## [Unreleased]

## [0.1.0] — 2026-09-24

### Added

- Initial release: UnityExplorer × gregCore compatibility bridge (MelonLoader plugin).
- Toggle-collision guard: detects UnityExplorer/Trainer shared hotkey from
  prefs and pre-seeds UnityExplorer's toggle with a free fallback (`Insert`)
  before it initializes; late re-verify with toast + instructions.
- Input-lock sync: UnityExplorer menu open → gregCore camera/movement/
  interact locks + cursor via a registered `unityexplorer` menu; released on
  close, disable, and unload.
- F1-hub integration: open/close UnityExplorer from the hub, HUD hint with
  the live toggle key, F1 config entries (`Enabled`, `SyncInputLocks`).
- Reflection-only UnityExplorer link (`UIManager.ShowMenu` get/set): missing
  or renamed APIs degrade to idle logging, never errors. No gregCore needed
  for the pre-seed guard (soft dependency, JIT-split).
- Docs: `docs/USAGE.md` (recommended UnityExplorer settings), `docs/ARCHITECTURE.md`,
  `docs/COMPATIBILITY.md`.
