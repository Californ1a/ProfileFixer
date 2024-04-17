using System.Collections.Generic;

namespace ProfileFixer.Utils
{
    /// <summary>
    /// Static utilities class for common functions and properties to be used within your mod code
    /// </summary>
    internal static class Ask
    {
        internal static void TOS(ProfileProgress progress)
        {
            G.Sys.MenuPanelManager_.ShowYesNo(
                "Restore The Other Side?\n\n(Requires having The Other Side achievement)",
                "The Other Side",
                () =>
                {
                    if (G.Sys.Achievements_.HasAchieved(EAchievements.TheOtherSide))
                    {
                        Unlock.TOS(progress);
                    }
                    else
                    {
                        ProfileFixerPlugin.Log.LogInfo("Player does not have The Other Side achievement.");
                    }
                    LBCheck.Check(progress);
                }, () =>
                {
                    LBCheck.Check(progress);
                });
        }

        internal static void Nexus(ProfileProgress progress)
        {
            G.Sys.MenuPanelManager_.ShowYesNo(
                "Restore Nexus?\n\n(No achievement required)",
                "Nexus",
                () =>
                {
                    Unlock.Nexus(progress);
                    Ask.TOS(progress);
                }, () =>
                {
                    Ask.TOS(progress);
                });
        }

        internal static void LtE(ProfileProgress progress)
        {
            G.Sys.MenuPanelManager_.ShowYesNo(
                "Restore Lost to Echoes?\n\n(Requires having the Blast from the Past achievement)",
                "Lost to Echoes",
                () =>
                {
                    if (G.Sys.Achievements_.HasAchieved(EAchievements.BlastFromThePast))
                    {
                        Unlock.LtE(progress);
                    }
                    else
                    {
                        ProfileFixerPlugin.Log.LogInfo("Player does not have the Blast from the Past achievement.");
                    }
                    Ask.Nexus(progress);
                }, () =>
                {
                    Ask.Nexus(progress);
                });
        }

        internal static void Adventure(ProfileProgress progress)
        {
            G.Sys.MenuPanelManager_.ShowYesNo(
                "Restore Adventure?\n\n(Requires having the Adventurer achievement)",
                "Adventure",
                () =>
                {
                    if (G.Sys.Achievements_.HasAchieved(EAchievements.Adventurer))
                    {
                        Unlock.Adventure(progress);
                    }
                    else
                    {
                        ProfileFixerPlugin.Log.LogInfo("Player does not have the Adventurer achievement.");
                    }
                    Ask.LtE(progress);
                }, () =>
                {
                    Ask.LtE(progress);
                });
        }

        internal static void UseAchievements(ProfileProgress progress)
        {
            G.Sys.MenuPanelManager_.ShowYesNo(
                "Use achievements to automatically restore campaigns?", // \n\nThis will skip restoring Nexus, since it has no achievement.",
                "Use Achievements?",
                () =>
                {
                    bool adventurer = G.Sys.Achievements_.HasAchieved(EAchievements.Adventurer);
                    bool blast = G.Sys.Achievements_.HasAchieved(EAchievements.BlastFromThePast);
                    bool tos = G.Sys.Achievements_.HasAchieved(EAchievements.TheOtherSide);
                    if (adventurer)
                    {
                        Unlock.Adventure(progress);
                    }
                    if (blast)
                    {
                        Unlock.LtE(progress);
                    }
                    if (tos)
                    {
                        Unlock.TOS(progress);
                    }

                    /*List<LevelNameAndPathPair> nexusSprint = G.Sys.LevelSets_.GetSet(GameModeID.Sprint).GetLevelNameAndPathPairs(LevelGroupFlags.Resource).FindAll(lvl =>
                    {
                        return lvl.levelName_ == "Resonance" || lvl.levelName_ == "Deterrence" || lvl.levelName_ == "Terminus";
                    });*/

                    LevelSet nexusSet = G.Sys.LevelSets_.GetNexusSet();
                    List<LevelNameAndPathPair> nexusList = nexusSet.GetAllLevelNameAndPathPairs();

                    bool unlockNexus = false;
                    foreach (LevelNameAndPathPair lvl in nexusList)
                    {
                        MedalStatus status = Status.GetMedalStatusBasedOnLeaderboard(lvl.levelPath_, GameModeID.Sprint);
                        if (status == MedalStatus.Completed ||
                            status == MedalStatus.Bronze ||
                            status == MedalStatus.Silver||
                            status == MedalStatus.Gold ||
                            status == MedalStatus.Diamond)
                        {
                            unlockNexus = true;
                            break;
                        }
                    }
                    if (unlockNexus)
                    {
                        Unlock.Nexus(progress);
                    }

                    LBCheck.Check(progress);
                }, () =>
                {
                    Ask.Adventure(progress);
                });
        }
    }
}