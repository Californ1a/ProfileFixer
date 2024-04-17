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
            MedalStatus status = MedalStatus.None;
            Profile currentProfile = G.Sys.ProfileManager_.CurrentProfile_;
            LevelInfo levelInfo = G.Sys.LevelSets_.GetLevelInfo(levelPath);
            List<ResultInfo> list = null;
            LocalLeaderboard localLeaderboard = LocalLeaderboard.Load(levelPath, modeID);
            if (localLeaderboard != null)
            {
                list = new List<ResultInfo>(localLeaderboard.Results_);
            }
            if (list != null && list.Count > 0)
            {
                bool flag = false;
                if (modeID.IsTimeBased())
                {
                    int num = int.MaxValue;
                    foreach (ResultInfo resultInfo in list)
                    {
                        if (resultInfo.ProfileID_ == currentProfile.ProfileID_ || resultInfo.ProfileName_ == currentProfile.Name_)
                        {
                            flag = true;
                            if (resultInfo.Value_ < num)
                            {
                                num = resultInfo.Value_;
                            }
                        }
                    }
                    if (flag)
                    {
                        status = GameMode.EvaluateMedalStatus(modeID, levelInfo, (double)num);
                    }
                }
                else if (modeID.IsPointsBased())
                {
                    int num2 = -2;
                    foreach (ResultInfo resultInfo2 in list)
                    {
                        if (resultInfo2.ProfileID_ == currentProfile.ProfileID_ || resultInfo2.ProfileName_ == currentProfile.Name_)
                        {
                            flag = true;
                            if (resultInfo2.Value_ > num2)
                            {
                                num2 = resultInfo2.Value_;
                            }
                        }
                    }
                    if (flag)
                    {
                        status = GameMode.EvaluateMedalStatus(modeID, levelInfo, (double)num2);
                    }
                }
            }
            if (localLeaderboard != null)
            {
                UnityEngine.Object.Destroy(localLeaderboard.gameObject);
            }
            return status;
        }
    }
}