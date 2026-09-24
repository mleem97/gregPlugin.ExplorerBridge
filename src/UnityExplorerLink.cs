using System;
using System.Reflection;
using MelonLoader;

namespace GregPlugin.ExplorerBridge
{
    /// <summary>
    /// Version-tolerant link to UnityExplorer via reflection. No compile-time
    /// reference (UnityExplorer ships separately through the Workshop) — every
    /// lookup is guarded, and a missing/renamed API degrades to idle logging.
    ///
    /// Uses: UIManager.ShowMenu (get = menu open?, set = toggle from F1 hub).
    /// </summary>
    internal static class UnityExplorerLink
    {
        private const string UiManagerType = "UnityExplorer.UI.UIManager";

        private static bool _probed;
        private static bool _available;
        private static PropertyInfo _showMenu;
        private static string _version = "?";

        internal static bool Available
        {
            get
            {
                if (!_probed) Probe();
                return _available;
            }
        }

        internal static string Version => _version;

        private static void Probe()
        {
            _probed = true;
            try
            {
                Assembly found = null;
                try
                {
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        string n;
                        try { n = asm.GetName().Name ?? ""; }
                        catch { continue; }
                        if (n.StartsWith("UnityExplorer", StringComparison.OrdinalIgnoreCase))
                        {
                            found = asm;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[ExplorerBridge] Assembly scan failed: {ex.Message}");
                    return;
                }
                if (found == null) return; // UnityExplorer not installed: idle

                Type ui;
                try { ui = found.GetType(UiManagerType); }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[ExplorerBridge] UIManager lookup failed: {ex.Message}");
                    return;
                }
                if (ui == null)
                {
                    MelonLogger.Warning("[ExplorerBridge] UnityExplorer.UI.UIManager not found (unsupported UE build?) — bridge idles.");
                    return;
                }

                PropertyInfo prop;
                try { prop = ui.GetProperty("ShowMenu", BindingFlags.Public | BindingFlags.Static); }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[ExplorerBridge] ShowMenu lookup failed: {ex.Message}");
                    return;
                }
                if (prop == null || !prop.CanRead)
                {
                    MelonLogger.Warning("[ExplorerBridge] UIManager.ShowMenu unreadable — bridge idles.");
                    return;
                }

                _showMenu = prop;
                _available = true;
                try
                {
                    var info = found.GetName();
                    _version = info.Version != null ? info.Version.ToString() : "?";
                }
                catch { /* keep "?" */ }
                MelonLogger.Msg($"[ExplorerBridge] Linked UnityExplorer (assembly v{_version}).");
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[ExplorerBridge] Probe failed: {ex.Message}");
            }
        }

        /// <summary>Re-probe (UnityExplorer loads after plugins).</summary>
        internal static void Reprobe()
        {
            _probed = false;
            _available = false;
            _showMenu = null;
            Probe();
        }

        internal static bool? ReadMenuOpen()
        {
            try
            {
                if (!Available || _showMenu == null) return null;
                object v = _showMenu.GetValue(null, null);
                if (v is bool b) return b;
                return null;
            }
            catch { return null; }
        }

        internal static bool TrySetMenuOpen(bool open)
        {
            try
            {
                if (!Available || _showMenu == null || !_showMenu.CanWrite) return false;
                _showMenu.SetValue(null, open, null);
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[ExplorerBridge] Menu toggle failed: {ex.Message}");
                return false;
            }
        }

        internal static bool TryToggleMenu()
        {
            try
            {
                bool? cur = ReadMenuOpen();
                if (!cur.HasValue) return false;
                return TrySetMenuOpen(!cur.Value);
            }
            catch { return false; }
        }
    }
}
