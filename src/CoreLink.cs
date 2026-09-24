using System;
using MelonLoader;

namespace GregPlugin.ExplorerBridge
{
    /// <summary>
    /// All direct gregCore references live in this class and this class only.
    /// Called exclusively behind <see cref="GregHost.HasCore"/> (JIT split),
    /// so the bridge loads and pre-seeds preferences without gregCore.
    ///
    /// Covers: F1 config entries, mod registry, HUD hint, F1-hub open/close
    /// for UnityExplorer, input-lock reporting, and toast notifications.
    /// </summary>
    internal static class CoreLink
    {
        internal const string MenuId = "unityexplorer";

        internal static void RegisterEntries()
        {
            DataCenterModLoader.ModConfigSystem.RegisterBool(
                BridgeConfig.ModId, "Enabled", "Bridge enabled", true,
                "Master switch: input-lock sync, F1 hub entry, toggle-collision guard.");
            DataCenterModLoader.ModConfigSystem.RegisterBool(
                BridgeConfig.ModId, "SyncInputLocks", "Sync input locks", true,
                "While UnityExplorer's menu is open, hold gregCore input locks (camera/movement/interact) + cursor.");
        }

        internal static bool GetBoolValue(string modId, string key, bool fallback)
        {
            try { return DataCenterModLoader.ModConfigSystem.GetBoolValue(modId, key, fallback); }
            catch { return fallback; }
        }

        internal static void RegisterHub(string version, Func<string> toggleKeyText, Func<bool> toggleMenu)
        {
            try
            {
                gregCore.Core.Mods.GregModRegistry.Register(
                    BridgeConfig.ModId, "ExplorerBridge", version,
                    new string[] { "explorerbridge" });
                string keyText = "F7";
                try { keyText = toggleKeyText != null ? (toggleKeyText() ?? "F7") : "F7"; }
                catch { /* default stands */ }
                gregCore.UI.GregHudRegistry.Register("explorerbridge", keyText, "Explorer");
                gregCore.UI.GregMenuRegistry.RegisterMenu(MenuId,
                    new gregCore.UI.GregMenuOptions
                    {
                        LockCamera = true,
                        LockMovement = true,
                        LockInteract = true,
                        ShowCursor = true,
                    });
                gregCore.UI.GregMenuRegistry.RegisterOpener(MenuId, () =>
                {
                    try { toggleMenu?.Invoke(); } catch { /* best-effort */ }
                });
                gregCore.UI.GregMenuRegistry.RegisterCloser(MenuId, () =>
                {
                    try { UnityExplorerLink.TrySetMenuOpen(false); } catch { /* best-effort */ }
                });
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[ExplorerBridge] Hub registration failed: " + ex.GetBaseException().Message);
            }
        }

        internal static void SetOpen(bool open)
        {
            try { gregCore.UI.GregMenuRegistry.SetOpen(MenuId, open); }
            catch { /* best-effort */ }
        }

        internal static void Toast(string message)
        {
            try { gregCore.UI.GregNotificationManager.Show(message, 6f); }
            catch
            {
                try { MelonLogger.Msg("[ExplorerBridge] " + message); }
                catch { /* best-effort */ }
            }
        }
    }
}
