using System.Collections.Generic;
using System;

namespace ProfileFixer.Utils
{
    /// <summary>
    /// Static utilities class for common functions and properties to be used within your mod code
    /// </summary>
    internal static class Unlock
    {
        private static void Set(LevelSet levelSet, GameModeID gameModeID, ProfileProgress progress)
        {
            List<LevelNameAndPathPair> allLevelNameAndPathPairs = levelSet.GetAllLevelNameAndPathPairs();
            foreach (LevelNameAndPathPair levelNameAndPathPair in allLevelNameAndPathPairs)
            {
                progress.UpdateMedal(levelNameAndPathPair.levelPath_, DateTime.Now, gameModeID, MedalStatus.Completed);
            }
        }

        internal static void Adventure(ProfileProgress progress)
        {
            LevelSet adventureSet = G.Sys.LevelSets_.GetAdventureSet();
            Unlock.Set(adventureSet, GameModeID.Adventure, progress);
        }

        internal static void LtE(ProfileProgress progress)
        {
            LevelSet echoesModeFinalSet = G.Sys.LevelSets_.GetEchoesModeFinalSet();
            Unlock.Set(echoesModeFinalSet, GameModeID.LostToEchoes, progress);
        }

        internal static void Nexus(ProfileProgress progress)
        {
            LevelSet nexusModeSet = G.Sys.LevelSets_.GetNexusSet();
            Unlock.Set(nexusModeSet, GameModeID.Nexus, progress);
        }

        internal static void TOS(ProfileProgress progress)
        {
            for (int i = 0; i < 8; i++)
            {
                progress.SetCrabAsFound(i);
            }
            LevelSet tosModeSet = G.Sys.LevelSets_.GetTheOtherSideSet();
            Unlock.Set(tosModeSet, GameModeID.TheOtherSide, progress);
        }
    }
}