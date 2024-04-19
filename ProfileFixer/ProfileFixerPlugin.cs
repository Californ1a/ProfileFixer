using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using ProfileFixer.Utils;
using ProfileFixer.Properties;

namespace ProfileFixer
{
    [BepInPlugin(ModInfo.MyGUID, ModInfo.PluginName, ModInfo.Version)]
    public class ProfileFixerPlugin : BaseUnityPlugin
    {

        internal static bool isChecking = false;

        // Config entry key strings
        // These will appear in the config file created by BepInEx and can also be used
        // by the OnSettingsChange event to determine which setting has changed.
        public static string KeyComboKey = "Shortcut";
        public static string MinFPSKey = "Min fps";

        // Configuration entries. Static, so can be accessed directly elsewhere in code via
        // e.g.
        // float myFloat = ProfileFixerPlugin.FloatExample.Value;
        public static ConfigEntry<KeyboardShortcut> KeyCombo;
        public static ConfigEntry<int> MinFPS;

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
            Log = BepInEx.Logging.Logger.CreateLogSource(ModInfo.PluginName);

            // Keyboard shortcut setting example
            KeyCode[] modifiers = new KeyCode[] { KeyCode.LeftControl, KeyCode.RightShift };
            KeyCombo = Config.Bind("General",
                KeyComboKey,
                new KeyboardShortcut(KeyCode.P, modifiers),
                "Key combo to start the profile checker. By default is intentionally hard to press.");

            MinFPS = Config.Bind("General",
                MinFPSKey,
                100,
                new ConfigDescription("The minimum FPS to target while scanning local leaderboards",
                    new AcceptableValueRange<int>(1, 60)));

            // Add listeners methods to run if and when settings are changed by the player.
            KeyCombo.SettingChanged += ConfigSettingChanged;
            MinFPS.SettingChanged += ConfigSettingChanged;

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
            if (KeyCombo.Value.IsDown())
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

            // Example Int Shortcut setting changed handler
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == MinFPSKey)
            {
                int newValue = (int)settingChangedEventArgs.ChangedSetting.BoxedValue;

            }

            // Example Keyboard Shortcut setting changed handler
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == KeyComboKey)
            {
                KeyboardShortcut newValue = (KeyboardShortcut)settingChangedEventArgs.ChangedSetting.BoxedValue;
            }
        }
    }
}
