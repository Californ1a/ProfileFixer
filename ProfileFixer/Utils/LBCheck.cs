using Events.OptionsMenu;
using Events;
using ProfileFixer.MonoBehaviors;
using UnityEngine;

namespace ProfileFixer.Utils
{
    /// <summary>
    /// Static utilities class for common functions and properties to be used within your mod code
    /// </summary>
    internal class LBCheck
    {
        internal static void Check(ProfileProgress progress)
        {
            G.Sys.MenuPanelManager_.ShowError(
                "This will now check local leaderboard files for matching profile name and attempt to restore obtained medals.\n\nIt may take a while, just wait.",
                "Check Local Leaderboards",
                () =>
                {
                    ProfileFixerPlugin.Log.LogInfo("Checking profile...");

                    G.Sys.MenuPanelManager_.MenuInputEnabled_ = false;

                    // This works but also automatically unlocks Adventure and LtE
                    // G.Sys.ProfileManager_.CurrentProfileProgress_.InitializeBasedOnLeaderboards();
                    // So we do just arcade medals manually...

                    GameObject progressBarObject = new GameObject("LBProgressBarObject");
                    LBProgressBar progressBar = progressBarObject.AddComponent<LBProgressBar>();
                    ProfileFixerPlugin.Log.LogInfo("Initialized");

                    GameObject loadingGameObject = Resource.LoadPrefabInstance("SteamWorkshopLoadingText", true);
                    SteamWorkshopLoadingText progressText = loadingGameObject.GetComponent<SteamWorkshopLoadingText>();

                    progressBar.StartProcessing(progress, progressText, () =>
                    {
                        Object.Destroy(loadingGameObject);
                        G.Sys.MenuPanelManager_.MenuInputEnabled_ = true;

                        ProfileFixerPlugin.Log.LogInfo("Done profile check");
                        G.Sys.MenuPanelManager_.ShowError(
                            "Done checking local leaderboard files for missing medals.\n\nYour profile will be reloaded now.",
                            "Profile Progress Restored",
                            () =>
                            {
                                int currentProfileIndex_ = G.Sys.ProfileManager_.CurrentProfile_.Index_;
                                G.Sys.ProfileManager_.SetDefaultLocalProfileIndexAndCreatePlayer(currentProfileIndex_);

                                bool respawnMenuObjects = false;

                                G.Sys.GameManager_.GoToMainMenu(GameManager.OpenOnMainMenuInit.None);
                                StaticEvent<ProfileSelected.Data>.Broadcast(new ProfileSelected.Data(respawnMenuObjects));
                                ProfileFixerPlugin.isChecking = false;
                                ProfileFixerPlugin.Log.LogInfo("Profile Fixer complete");

                            });
                        Object.Destroy(progressBarObject);
                    });
                });
        }
    }
}