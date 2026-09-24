using System;
using MelonLoader;

[assembly: MelonInfo(typeof(GregPlugin.ExplorerBridge.ExplorerBridgePlugin),
    GregPlugin.ExplorerBridge.MyPluginInfo.PLUGIN_NAME,
    GregPlugin.ExplorerBridge.MyPluginInfo.PLUGIN_VERSION,
    GregPlugin.ExplorerBridge.MyPluginInfo.PLUGIN_AUTHOR)]
[assembly: MelonGame("Waseku", "Data Center")]

namespace GregPlugin.ExplorerBridge
{
    /// <summary>
    /// gregPlugin.ExplorerBridge — makes UnityExplorer and gregCore run smoothly together.
    ///
    /// 1. Toggle-collision guard: UnityExplorer (default F7) and gregMod.Trainer
    ///    (default F7) fight over one key. Before UnityExplorer initializes
    ///    (plugins load first), the bridge pre-seeds UnityExplorer's toggle
    ///    preference with a free fallback key when a collision is detected.
    /// 2. Input-lock sync: while UnityExplorer's menu is open, gregCore holds
    ///    camera/movement/interact locks + cursor (no camera spin while
    ///    clicking explorer panels, no cursor fights).
    /// 3. F1-hub integration: open/close UnityExplorer from the gregCore hub,
    ///    HUD hint with the live toggle key, F1 config entries.
    ///
    /// UnityExplorer is linked by reflection only (ships separately via the
    /// Workshop): missing or renamed APIs degrade to idle logging, never errors.
    /// Without gregCore, pre-seeding still works; lock sync is skipped.
    /// </summary>
    public sealed class ExplorerBridgePlugin : MelonPlugin
    {
        private static int _frame;
        private static bool _lastMenuOpen;
        private static bool _linkLogged;
        private static bool _reverified;
        private static bool _locksReleased = true;

        public override void OnInitializeMelon()
        {
            try
            {
                BridgeConfig.Load();
                TryPreseedToggleFix();

                if (GregHost.HasCore)
                {
                    try
                    {
                        BridgeConfig.RegisterF1Entries();
                        CoreLink.RegisterHub(
                            MyPluginInfo.PLUGIN_VERSION,
                            ReadUeToggleKeyText,
                            UnityExplorerLink.TryToggleMenu);
                    }
                    catch (Exception ex)
                    {
                        LoggerInstance.Warning($"[ExplorerBridge] gregCore wiring failed: {ex.GetBaseException().Message}");
                    }
                }

                LoggerInstance.Msg(
                    $"[ExplorerBridge] {MyPluginInfo.PLUGIN_VERSION} loaded. " +
                    $"Enabled={BridgeConfig.Enabled}, SyncLocks={BridgeConfig.SyncInputLocks}. " +
                    "UnityExplorer linked lazily when it appears.");
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"[ExplorerBridge] Startup failed: {ex.GetBaseException().Message}");
            }
        }

        public override void OnUpdate()
        {
            _frame++;
            try
            {
                // Lazy link: UnityExplorer (a mod) initializes after plugins.
                if (!UnityExplorerLink.Available)
                {
                    if (_frame % 300 == 1) UnityExplorerLink.Reprobe();
                    return;
                }
                if (!_linkLogged)
                {
                    _linkLogged = true;
                    LoggerInstance.Msg($"[ExplorerBridge] Tracking UnityExplorer menu (v{UnityExplorerLink.Version}).");
                }

                // One-time late collision re-verify (both mods initialized by now).
                if (!_reverified && _frame > 60 * 15)
                {
                    _reverified = true;
                    try { ReverifyCollision(); }
                    catch (Exception ex)
                    {
                        LoggerInstance.Warning($"[ExplorerBridge] Collision re-verify failed: {ex.Message}");
                    }
                }

                if (!BridgeConfig.Enabled)
                {
                    ReleaseLocksOnce();
                    return;
                }

                bool? open = UnityExplorerLink.ReadMenuOpen();
                if (!open.HasValue) return;
                if (open.Value == _lastMenuOpen) return;
                _lastMenuOpen = open.Value;

                if (GregHost.HasCore && BridgeConfig.SyncInputLocks)
                {
                    try { CoreLink.SetOpen(open.Value); }
                    catch { /* best-effort */ }
                }
                LoggerInstance.Msg($"[ExplorerBridge] UnityExplorer menu {(open.Value ? "OPEN — input locks held" : "closed — locks released")}.");
            }
            catch { /* polling best-effort */ }
        }

        public override void OnDeinitializeMelon()
        {
            try { ReleaseLocksOnce(force: true); }
            catch { /* best-effort */ }
        }

        private static void ReleaseLocksOnce(bool force = false)
        {
            if (_locksReleased && !force) return;
            _locksReleased = true;
            _lastMenuOpen = false;
            try
            {
                if (GregHost.HasCore) CoreLink.SetOpen(false);
            }
            catch { /* best-effort */ }
        }

        // ── Toggle-collision guard ───────────────────────────────────────────

        private void TryPreseedToggleFix()
        {
            try
            {
                if (!BridgeConfig.AutoFixToggleCollision) return;

                string ueKey = BridgeConfig.ReadPrefFile(BridgeConfig.UeCategory,
                    BridgeConfig.UeToggleIdentifier, BridgeConfig.UeToggleDefault);
                string trainerKey = BridgeConfig.ReadPrefFile(BridgeConfig.TrainerCategory,
                    BridgeConfig.TrainerToggleIdentifier, BridgeConfig.TrainerToggleDefault);

                if (!string.Equals(ueKey, trainerKey, StringComparison.OrdinalIgnoreCase))
                    return; // no collision

                string fallback = BridgeConfig.UeFallbackKey;
                if (string.Equals(fallback, trainerKey, StringComparison.OrdinalIgnoreCase))
                {
                    LoggerInstance.Warning($"[ExplorerBridge] Toggle collision ({ueKey}) but fallback '{fallback}' collides too — set UeFallbackKey manually.");
                    return;
                }

                // Pre-seed BEFORE UnityExplorer initializes (plugins load first):
                // its CreateEntry then adopts our value instead of the default.
                // If the entry already exists (unexpected order), rewrite it live.
                bool preseeded = false;
                try
                {
                    if (!MelonPreferences.HasEntry(BridgeConfig.UeCategory, BridgeConfig.UeToggleIdentifier))
                    {
                        var cat = MelonPreferences.CreateCategory(BridgeConfig.UeCategory, "Unity Explorer");
                        var entry = cat.CreateEntry(BridgeConfig.UeToggleIdentifier, fallback,
                            "Toggle key (managed)",
                            "Moved aside by gregPlugin.ExplorerBridge: shared with gregMod.Trainer otherwise.");
                        preseeded = entry != null;
                    }
                    else
                    {
                        var typed = MelonPreferences.GetCategory(BridgeConfig.UeCategory)
                            .GetEntry<string>(BridgeConfig.UeToggleIdentifier);
                        if (typed != null)
                        {
                            typed.Value = fallback;
                            preseeded = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerInstance.Warning($"[ExplorerBridge] Toggle write failed (harmless): {ex.GetBaseException().Message}");
                }
                try { MelonPreferences.Save(); } catch { /* best-effort */ }

                LoggerInstance.Msg($"[ExplorerBridge] Toggle collision fixed: UnityExplorer {ueKey} -> {fallback} " +
                    $"(Trainer keeps {trainerKey}). Applied pre-startup: {preseeded}. " +
                    "If UnityExplorer ignores it, restart once.");
            }
            catch (Exception ex)
            {
                LoggerInstance.Warning($"[ExplorerBridge] Toggle pre-seed failed (harmless): {ex.GetBaseException().Message}");
            }
        }

        private void ReverifyCollision()
        {
            try
            {
                string ueKey = ReadLiveEntry(BridgeConfig.UeCategory, BridgeConfig.UeToggleIdentifier)
                    ?? BridgeConfig.ReadPrefFile(BridgeConfig.UeCategory, BridgeConfig.UeToggleIdentifier, BridgeConfig.UeToggleDefault);
                string trainerKey = ReadLiveEntry(BridgeConfig.TrainerCategory, BridgeConfig.TrainerToggleIdentifier)
                    ?? BridgeConfig.ReadPrefFile(BridgeConfig.TrainerCategory, BridgeConfig.TrainerToggleIdentifier, BridgeConfig.TrainerToggleDefault);

                if (!string.Equals(ueKey, trainerKey, StringComparison.OrdinalIgnoreCase))
                    return;

                string msg = $"Toggle collision still active: UnityExplorer + Trainer share '{ueKey}'. " +
                    "Change one toggle key (UE prefs section or Trainer F1 entry); restart may be needed.";
                LoggerInstance.Warning("[ExplorerBridge] " + msg);
                if (GregHost.HasCore)
                {
                    try { CoreLink.Toast(msg); } catch { /* best-effort */ }
                }
            }
            catch (Exception ex)
            {
                LoggerInstance.Warning($"[ExplorerBridge] Re-verify failed: {ex.Message}");
            }
        }

        private static string ReadLiveEntry(string category, string identifier)
        {
            try
            {
                if (!MelonPreferences.HasEntry(category, identifier)) return null;
                var entry = MelonPreferences.GetEntry(category, identifier);
                if (entry == null) return null;
                object boxed;
                try { boxed = entry.BoxedValue; }
                catch { return null; }
                return boxed != null ? boxed.ToString() : null;
            }
            catch { /* best-effort */ }
            return null;
        }

        private static string ReadUeToggleKeyText()
        {
            try
            {
                return ReadLiveEntry(BridgeConfig.UeCategory, BridgeConfig.UeToggleIdentifier)
                    ?? BridgeConfig.ReadPrefFile(BridgeConfig.UeCategory, BridgeConfig.UeToggleIdentifier, BridgeConfig.UeToggleDefault);
            }
            catch { return BridgeConfig.UeToggleDefault; }
        }
    }
}
