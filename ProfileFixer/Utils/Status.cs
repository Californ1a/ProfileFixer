using System.Collections.Generic;

namespace ProfileFixer.Utils
{
    /// <summary>
    /// Static utilities class for common functions and properties to be used within your mod code
    /// </summary>
    internal static class Status
    {
        internal static MedalStatus GetMedalStatusBasedOnLeaderboard(string levelPath, GameModeID modeID)
        {
            MedalStatus medalStatus = MedalStatus.None;
            Profile currentProfile = G.Sys.ProfileManager_.CurrentProfile_;
            LevelInfo levelInfo = G.Sys.LevelSets_.GetLevelInfo(levelPath);
            List<ResultInfo> resultsList = null;
            LocalLeaderboard localLeaderboard = LocalLeaderboard.Load(levelPath, modeID);
            if (localLeaderboard != null)
            {
                resultsList = new List<ResultInfo>(localLeaderboard.Results_);
            }
            if (resultsList != null && resultsList.Count > 0)
            {
                bool hasValidResult = false;
                if (modeID.IsTimeBased())
                {
                    int minTime = int.MaxValue;
                    foreach (ResultInfo resultInfo in resultsList)
                    {
                        if (resultInfo.ProfileID_ == currentProfile.ProfileID_ || resultInfo.ProfileName_ == currentProfile.Name_)
                        {
                            hasValidResult = true;
                            if (resultInfo.Value_ < minTime)
                            {
                                minTime = resultInfo.Value_;
                            }
                        }
                    }
                    if (hasValidResult)
                    {
                        medalStatus = GameMode.EvaluateMedalStatus(modeID, levelInfo, (double)minTime);
                    }
                }
                else if (modeID.IsPointsBased())
                {
                    int maxPoints = -2;
                    foreach (ResultInfo resultInfo in resultsList)
                    {
                        if (resultInfo.ProfileID_ == currentProfile.ProfileID_ || resultInfo.ProfileName_ == currentProfile.Name_)
                        {
                            hasValidResult = true;
                            if (resultInfo.Value_ > maxPoints)
                            {
                                maxPoints = resultInfo.Value_;
                            }
                        }
                    }
                    if (hasValidResult)
                    {
                        medalStatus = GameMode.EvaluateMedalStatus(modeID, levelInfo, (double)maxPoints);
                    }
                }
            }
            if (localLeaderboard != null)
            {
                UnityEngine.Object.Destroy(localLeaderboard.gameObject);
            }
            return medalStatus;
        }
    }
}