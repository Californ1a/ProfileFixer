using Events.OptionsMenu;
using Events;
using System.Collections.Generic;
using System.Reflection;

namespace ProfileFixer.Utils
{
    /// <summary>
    /// Static utilities class for common functions and properties to be used within your mod code
    /// </summary>
    internal static class LBCheck
    {
        internal static void Check(ProfileProgress progress)
        {
            G.Sys.MenuPanelManager_.ShowError(
                "This will now check local leaderboard files\nfor matching profile name and\nattempt to restore obtained medals.",
                "Check Local Leaderboards",
                () =>
                {
                    ProfileFixerPlugin.Log.LogInfo("Checking profile...");

                    // This works but also automatically unlocks Adventure and LtE
                    // G.Sys.ProfileManager_.CurrentProfileProgress_.InitializeBasedOnLeaderboards();
                    // So we do just arcade medals manually...

                    // Manually only check for arcade medals
                    GameManager gameManager_ = G.Sys.GameManager_;
                    HashSet<string> allLevelPaths_ = G.Sys.LevelSets_.AllLevelPaths_;

                    // Reflection for private method
                    MethodInfo updateMedalStatus = progress.GetType().GetMethod(
                        "UpdateMedalStatusBasedOnLeaderboard", BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (GameModeID modeID in gameManager_.ModeIDs_)
                    {
                        foreach (string levelPath in allLevelPaths_)
                        {
                            // Replaces the following private method call
                            // currentProgress.UpdateMedalStatusBasedOnLeaderboard(levelPath, modeID);
                            object[] methodParams = new object[] { levelPath, modeID };
                            updateMedalStatus.Invoke(progress, methodParams);
                        }
                    }
                    progress.UpdateUnlockedLevels();
                    progress.Save();

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
                        });
                });
        }
    }
}