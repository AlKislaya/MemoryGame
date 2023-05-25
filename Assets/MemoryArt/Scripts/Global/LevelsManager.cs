using System;
using System.Collections.Generic;
using System.Linq;
using MemoryArt.Game.Levels;
using MemoryArt.Global.Patterns;
using UnityEngine;
using YG;

namespace MemoryArt.Global
{
    public class LevelsManager : Singleton<LevelsManager>
    {
        [SerializeField] private LevelsCategory _baseLevelsCategory;
        [SerializeField] private List<LevelsCategory> _levelsCategories;
#if UNITY_WEBGL
        private Dictionary<string, LevelsProgress> _levelsProgress => YandexGame.savesData.LevelsProgress;
#else
        private Dictionary<string, LevelsProgress> _levelsProgress = new Dictionary<string, LevelsProgress>();
#endif
        public string BaseLevelsKey => _baseLevelsCategory.Key;

        //all categories except base
        public List<LevelsCategory> LevelsCategories => _levelsCategories;

        public bool IsCategoryProgressExists(string categoryKey)
        {
#if UNITY_WEBGL
            return _levelsProgress.ContainsKey(categoryKey);
#else
            return PlayerPrefs.HasKey(categoryKey);
#endif
        }

        public LevelsCategory GetCategoryByKey(string categoryKey)
        {
            return _baseLevelsCategory.Key.Equals(categoryKey)
                ? _baseLevelsCategory
                : _levelsCategories.FirstOrDefault(x => x.Key.Equals(categoryKey));
        }

        //get progress from dictionary or get json from Player Prefs and save to dictionary
        public LevelsProgress GetLevelsProgressByCategory(string categoryKey)
        {
#if UNITY_WEBGL
            if (!IsCategoryProgressExists(categoryKey)) {
                InitLevelsProgress(categoryKey);
            }
#else
            if (!_levelsProgress.ContainsKey(categoryKey))
            {
                var levelsJson = PlayerPrefs.GetString(categoryKey);

                if (string.IsNullOrEmpty(levelsJson))
                {
                    InitLevelsProgress(categoryKey);
                }
                else
                {
                    try
                    {
                        _levelsProgress.Add(categoryKey, JsonUtility.FromJson<LevelsProgress>(levelsJson));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        InitLevelsProgress(categoryKey);
                    }
                }
            }
#endif
            return _levelsProgress[categoryKey];
        }

        private void InitLevelsProgress(string categoryKey)
        {
            var levelsProgress = new LevelsProgress
            {
                Levels = new List<LevelProgress>() { new LevelProgress() { IsPassed = false } }
            };
            _levelsProgress.Add(categoryKey, levelsProgress);
            
            SaveLevelsProgress(categoryKey);
        }

        public void SetPassedLevel(string categoryKey, int levelIndex, float passedPercents)
        {
            var levelsProgress = GetLevelsProgressByCategory(categoryKey);

            var passedLevel = levelsProgress.Levels[levelIndex];
            passedLevel.PassedPercents = passedPercents;
            passedLevel.IsPassed = true;

            SaveLevelsProgress(categoryKey);
        }

        public void SetNewLevelProgress(string categoryKey)
        {
            _levelsProgress[categoryKey].Levels.Add(new LevelProgress() { IsPassed = false });
            SaveLevelsProgress(categoryKey);
        }

        private void SaveLevelsProgress(string categoryKey)
        {
#if UNITY_WEBGL
            YandexGame.SaveProgress();
            Debug.Log("SAVED! " + JsonUtility.ToJson(_levelsProgress[categoryKey]));
#else
            PlayerPrefs.SetString(categoryKey, JsonUtility.ToJson(_levelsProgress[categoryKey]));
            Debug.Log("SAVED! " + categoryKey + " : " + JsonUtility.ToJson(_levelsProgress[categoryKey]));
#endif
        }
    }
}