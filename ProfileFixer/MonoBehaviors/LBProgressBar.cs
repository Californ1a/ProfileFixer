using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProfileFixer.MonoBehaviors
{
    public class LBProgressBar : MonoBehaviour
    {
        private Action onProcessComplete;
        private GameManager gameManager;
        private List<string> levelPathsList;
        private MethodInfo updateMedalStatus;
        private ProfileProgress progress;
        private SteamWorkshopLoadingText progressText;

        //private readonly int batchSize = ProfileFixerPlugin.BatchSize.Value;

        private int currentIndex = 0;
        private int total = 0;
        private int maxChecksPerFrame = 200; // Initial number of checks per frame
        private float targetFrameRate = 60f; // Target frame rate
        private float minFrameTime = 1f / (float)ProfileFixerPlugin.MinFPS.Value; // Minimum frame time
        private int maxChecksIncrement = 20; // Initial increment for increasing max checks per frame
        private float decreaseRate = 0.1f; // Rate of decrease for increment

        public void StartProcessing(ProfileProgress progress, SteamWorkshopLoadingText progressText, Action onProcessComplete)
        {
            ProfileFixerPlugin.Log.LogInfo("StartProcessing");
            this.onProcessComplete = onProcessComplete;
            this.progress = progress;
            this.progressText = progressText;
            this.progressText.steamWorkshopProgressTextLabel_.text = "Loading...";
            this.progressText.steamWorkshopProgressBar_.value = 0;

            this.gameManager = G.Sys.GameManager_;
            LevelSetsManager levelSetsManager = G.Sys.LevelSets_;

            this.updateMedalStatus = this.progress.GetType().GetMethod(
                "UpdateMedalStatusBasedOnLeaderboard", BindingFlags.NonPublic | BindingFlags.Instance);

             this.levelPathsList = new List<string>(levelSetsManager.AllLevelPaths_);

            // Log counts
            ProfileFixerPlugin.Log.LogInfo("ModeIDs count: " + this.gameManager.ModeIDs_.Count);
            ProfileFixerPlugin.Log.LogInfo("LevelPaths count: " + this.levelPathsList.Count);

            this.total = this.gameManager.ModeIDs_.Count * this.levelPathsList.Count;

            StartCoroutine(ProcessMedalStatus());
        }

        private string GetNewLabel(int index, int count)
        {
            string lowerUpdateText = "\n[c][AAAAAA]Checking leaderboards...[-][/c]";
            string loadingPrefix = string.Concat(new object[]
            {
                "Progress ",
                (index + 1).ToString("N0"),
                " / ",
                count.ToString("N0")
            });

            int percentDone = (int)((float)index / (float)count * 100f);
			string ret = string.Concat(new object[]
			{
				loadingPrefix,
				" : ",
                percentDone,
				"%[-][/c]",
				lowerUpdateText
            });

            return ret;
        }

        private void UpdateProgress(float value, string labelText)
        {
            progressText.steamWorkshopProgressBar_.value = value;
            progressText.steamWorkshopProgressTextLabel_.text = labelText;
        }

        private IEnumerator ProcessMedalStatus()
        {
            ProfileFixerPlugin.Log.LogInfo("ProcessMedalStatus");

            while (currentIndex < total)
            {
                float frameStartTime = Time.realtimeSinceStartup;

                int checksThisFrame = 0;

                while (checksThisFrame < maxChecksPerFrame && currentIndex < total)
                {
                    GameModeID modeID = gameManager.ModeIDs_[currentIndex / levelPathsList.Count];
                    string levelPath = levelPathsList[currentIndex % levelPathsList.Count];

                    object[] methodParams = new object[] { levelPath, modeID };
                    updateMedalStatus.Invoke(progress, methodParams);

                    float progressValue = (float)(currentIndex + 1) / total;
                    UpdateProgress(progressValue, GetNewLabel(currentIndex + 1, total));

                    currentIndex++;
                    checksThisFrame++;
                }

                // Calculate time taken for the frame
                float frameTime = Time.realtimeSinceStartup - frameStartTime;

                // Adjust max checks per frame based on frame time and target frame rate
                if (frameTime < minFrameTime)
                {
                    maxChecksPerFrame += maxChecksIncrement;
                }
                else if (frameTime > 1f / targetFrameRate && maxChecksPerFrame > 1)
                {
                    maxChecksPerFrame -= maxChecksIncrement;
                }

                // Adjust the increment gradually over time
                if (maxChecksIncrement > 1)
                {
                    maxChecksIncrement = Mathf.Max(1, Mathf.RoundToInt(maxChecksIncrement * (1 - decreaseRate)));
                }

                yield return null; // Yield to let Unity render the frame
            }

            // All leaderboards checked, do any necessary cleanup or post-processing here




            this.progress.UpdateUnlockedLevels();
            this.progress.Save();

            this.onProcessComplete?.Invoke();
            yield break;
        }
    }
}
