using System;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;

namespace GregPlugin.ExplorerBridge
{
    /// <summary>
    /// Configuration for gregPlugin.ExplorerBridge.
    ///
    /// MelonPreferences (always) + gregCore F1 entries when present (F1 wins
    /// at read time for Enabled/SyncInputLocks). Key names stay prefs-only:
    /// the F1 UI exposes bool/int/float entries, and the UE toggle key must
    /// be written before UnityExplorer initializes (plugins load first).
    /// </summary>
    internal static class BridgeConfig
    {
        internal const string ModId = "gregPlugin.ExplorerBridge";

        // Known-good defaults observed in the wild.
        internal const string UeCategory = "UnityExplorer";
        internal const string UeToggleIdentifier = "UnityExplorer Toggle";
        internal const string UeToggleDefault = "F7";
        internal const string TrainerCategory = "gregMod.Trainer";
        internal const string TrainerToggleIdentifier = "ToggleKey";
        internal const string TrainerToggleDefault = "F7";

        private static MelonPreferences_Category _cat;
        private static MelonPreferences_Entry<bool> _enabled;
        private static MelonPreferences_Entry<bool> _syncInputLocks;
        private static MelonPreferences_Entry<bool> _autoFixToggleCollision;
        private static MelonPreferences_Entry<string> _ueFallbackKey;

        internal static void Load()
        {
            try
            {
                _cat = MelonPreferences.CreateCategory(ModId, "ExplorerBridge");
                _enabled = _cat.CreateEntry("Enabled", true, "Bridge enabled",
                    "Master switch: input-lock sync, F1 hub entry, toggle-collision guard.");
                _syncInputLocks = _cat.CreateEntry("SyncInputLocks", true, "Sync input locks",
                    "While UnityExplorer's menu is open, hold gregCore input locks (camera/movement/interact) + cursor.");
                _autoFixToggleCollision = _cat.CreateEntry("AutoFixToggleCollision", true, "Auto-fix toggle collision",
                    "If UnityExplorer and Trainer share one toggle key, move UnityExplorer's key aside before it initializes.");
                _ueFallbackKey = _cat.CreateEntry("UeFallbackKey", "Insert", "UE fallback toggle key",
                    "Replacement toggle key written into UnityExplorer's preferences when a collision is fixed.");
                _cat.SaveToFile(false);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[ExplorerBridge] Config load failed, using built-in defaults: {ex.GetBaseException().Message}");
            }
        }

        internal static bool Enabled => GetBool("Enabled", _enabled, true);

        internal static bool SyncInputLocks => GetBool("SyncInputLocks", _syncInputLocks, true);

        internal static bool AutoFixToggleCollision
        {
            get
            {
                try { return _autoFixToggleCollision != null ? _autoFixToggleCollision.Value : true; }
                catch { return true; }
            }
        }

        internal static string UeFallbackKey
        {
            get
            {
                try
                {
                    string v = _ueFallbackKey != null ? (_ueFallbackKey.Value ?? "") : "Insert";
                    return string.IsNullOrWhiteSpace(v) ? "Insert" : v.Trim();
                }
                catch { return "Insert"; }
            }
        }

        internal static void SetEnabled(bool v)
        {
            if (_enabled == null) return;
            try
            {
                _enabled.Value = v;
                MelonPreferences.Save();
            }
            catch { /* best-effort */ }
        }

        private static bool GetBool(string key, MelonPreferences_Entry<bool> pref, bool fallback)
        {
            if (GregHost.HasCore)
            {
                try { return CoreLink.GetBoolValue(ModId, key, pref != null ? pref.Value : fallback); }
                catch { /* fall through to prefs */ }
            }
            try { return pref != null ? pref.Value : fallback; }
            catch { return fallback; }
        }

        // ── Pref-file reading (works before any mod initializes) ─────────────

        /// <summary>
        /// Reads a raw "Key" value from MelonPreferences.cfg without requiring
        /// the owning mod to be initialized. Returns defaultValue when absent.
        /// </summary>
        internal static string ReadPrefFile(string category, string key, string defaultValue)
        {
            try
            {
                string path = Path.Combine(MelonEnvironment.UserDataDirectory, "MelonPreferences.cfg");
                if (!File.Exists(path)) return defaultValue;
                bool inCategory = false;
                foreach (string rawLine in File.ReadAllLines(path))
                {
                    string line;
                    try { line = rawLine.Trim(); }
                    catch { continue; }
                    if (line.Length == 0 || line.StartsWith("#")) continue;
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        inCategory = string.Equals(line.Substring(1, line.Length - 2).Trim(),
                            category, StringComparison.OrdinalIgnoreCase);
                        continue;
                    }
                    if (!inCategory) continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = line.Substring(0, eq).Trim().Trim('"');
                    if (!string.Equals(k, key, StringComparison.OrdinalIgnoreCase)) continue;
                    string v = line.Substring(eq + 1).Trim().Trim('"');
                    return v;
                }
            }
            catch { /* best-effort */ }
            return defaultValue;
        }

        internal static void RegisterF1Entries()
        {
            try { CoreLink.RegisterEntries(); }
            catch (Exception ex)
            {
                MelonLogger.Warning("[ExplorerBridge] F1 config registration failed: " + ex.GetBaseException().Message);
            }
        }
    }
}
