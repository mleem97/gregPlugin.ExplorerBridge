# AGENTS.md — Notes for AI agents (gregPlugin.ExplorerBridge)

Repo: [https://github.com/mleem97/gregPlugin.ExplorerBridge](https://github.com/mleem97/gregPlugin.ExplorerBridge) · License: Apache-2.0 · Version: see `VERSION`.

## Duties

1. **Read first:** `README.md`, `docs/INDEX.md`, `CONTRIBUTING.md` — only then make changes.
2. **Do not commit secrets** (keys, tokens, `.env`). Use keys only via environment variables.
3. **Preserve history:** no `push --force`, no history rewrite without instruction.
4. **Verify changes:** before reporting completion, build/test whatever the repo supports (`QUICKSTART.md`).
5. **Keep docs in sync:** for new features, update `README.md` + `docs/` + `CHANGELOG.md` (Unreleased).
6. **Conventions:** Conventional Commits (`feat:`, `fix:`, `docs:`, `chore:` …), one logical change per commit.
7. **When in doubt:** stop and ask instead of guessing — especially for deletes, migrations, CI.

## Plugin-specific rules

- **This is a MelonPlugin (`: MelonPlugin`), not a MelonMod.** It deploys to
  `Data Center/Plugins/`, loads before mods, and must never depend on mod
  load order. Keep the lazy link + retry logic.
- **Never reference UnityExplorer at compile time.** Reflection only
  (`UIManager.ShowMenu` get/set); probe failures idle out with a log line.
- **Never break third-party init.** The toggle pre-seed writes one pref value
  pre-init; everything is try/caught. If MelonLoader's duplicate-entry
  behavior ever changes, the bridge must still load harmlessly.
- **gregCore is a soft dependency.** Direct references live only in
  `src/CoreLink.cs` behind `GregHost.HasCore` (JIT split).
- **Don't steal keys blindly.** The fallback key must be free across Greg
  mods (F-keys are all taken: F3 Potato, F4 FiberTrunk, F5 NoEOL, F6
  Backplanes, F7 Trainer, F8 MultiCable, F9 MusicPlayer, F10 NotesHUD, F11
  MemeRoulette/Economics, F12 GameExport). Verify before changing defaults.
- **Reverse-engineering evidence** belongs in `docs/COMPATIBILITY.md`.

## Layout

See [README.md](README.md) → Repository Layout. Central entry points: `docs/INDEX.md`, `scripts/`, `tests/`.
Source: `src/` (`ExplorerBridgePlugin`, `BridgeConfig`, `UnityExplorerLink`,
`CoreLink`, `GregHost`, `MyPluginInfo`).
