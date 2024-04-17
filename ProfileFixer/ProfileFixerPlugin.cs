using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using ProfileFixer.Utils;

namespace ProfileFixer
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class ProfileFixerPlugin : BaseUnityPlugin
    {
        // Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
        // e.g.
        // com.mynameororg.pluginname
        // Version should be a valid version string.
        // e.g.
        // 1.0.0
        private const string MyGUID = "com.Californ1a.ProfileFixer";
        private const string PluginName = "ProfileFixer";
        private const string VersionString = "1.0.0";

        internal static bool isChecking = false;

        // Config entry key strings
        // These will appear in the config file created by BepInEx and can also be used
        // by the OnSettingsChange event to determine which setting has changed.
        public static string KeyboardShortcutExampleKey = "Shortcut";

        // Configuration entries. Static, so can be accessed directly elsewhere in code via
        // e.g.
        // float myFloat = ProfileFixerPlugin.FloatExample.Value;
        public static ConfigEntry<KeyboardShortcut> KeyboardShortcutExample;

        //private static readonly Harmony Harmony = new Harmony(MyGUID);
        internal static ManualLogSource Log;

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
            // Sets up our static Log, so it can be used elsewhere in code.
            // .e.g.
            // ProfileFixerPlugin.Log.LogDebug("Debug Message to BepInEx log file");
            Log = BepInEx.Logging.Logger.CreateLogSource(PluginName);

            // Keyboard shortcut setting example
            KeyCode[] modifiers = new KeyCode[] { KeyCode.LeftControl, KeyCode.RightShift };
            KeyboardShortcutExample = Config.Bind("General",
                KeyboardShortcutExampleKey,
                new KeyboardShortcut(KeyCode.P, modifiers),
                "Key combo to start the profile checker. By default is intentionally hard to press.");

            // Add listeners methods to run if and when settings are changed by the player.
            KeyboardShortcutExample.SettingChanged += ConfigSettingChanged;

            Log.LogInfo("Loaded");

        }

        private void RunProfileChecker()
        {
            if (isChecking) return; // Don't try to run the profile checker while it's still asking which modes to restore.
            if (!GameManager.IsInMainMenuScene_) return;
            ProfileProgress currentProgress = G.Sys.ProfileManager_.CurrentProfileProgress_;
            isChecking = true;
            Ask.UseAchievements(currentProgress);
        }

        /// <summary>
        /// Code executed every frame. See below for an example use case
        /// to detect keypress via custom configuration.
        /// </summary>
        private void Update()
        {
            if (KeyboardShortcutExample.Value.IsDown())
            {
                RunProfileChecker();
            }
        }

        /// <summary>
        /// Method to handle changes to configuration made by the player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigSettingChanged(object sender, System.EventArgs e)
        {
            SettingChangedEventArgs settingChangedEventArgs = e as SettingChangedEventArgs;

            // Check if null and return
            if (settingChangedEventArgs == null)
            {
                return;
            }

            // Example Keyboard Shortcut setting changed handler
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == KeyboardShortcutExampleKey)
            {
                KeyboardShortcut newValue = (KeyboardShortcut)settingChangedEventArgs.ChangedSetting.BoxedValue;

            }
        }
    }
}
