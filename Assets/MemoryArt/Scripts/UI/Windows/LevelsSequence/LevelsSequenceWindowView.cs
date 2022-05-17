using System;
using System.Collections.Generic;
using Dainty.UI.WindowBase;
using MemoryArt.Game.Levels;
using UnityEngine;

namespace MemoryArt.UI.Windows
{
    public class LevelsSequenceWindowView : AWindowView
    {
        [SerializeField] private LevelItem _levelPrefab;
        [SerializeField] private Transform _levelsContainer;

        private readonly List<LevelItem> _levelInstances = new List<LevelItem>();

        public event Action<int> LevelClicked;

        public void AddLevel(int levelIndex, bool isOpened, LevelProgress levelProgress = null, Sprite preview = null)
        {
            LevelItem levelInstance;
            if (_levelInstances.Count - 1 >= levelIndex)
            {
                levelInstance = _levelInstances[levelIndex];
            }
            else
            {
                levelInstance = Instantiate(_levelPrefab, _levelsContainer);
                levelInstance.Initialize(levelIndex, OnLevelClicked);
                _levelInstances.Add(levelInstance);
            }

            if (isOpened && levelProgress != null)
            {
                if (levelProgress.IsPassed)
                {
                    levelInstance.SetAsOpenedLevel(preview, levelProgress.PassedPercents);
                }
                else
                {
                    levelInstance.SetAsNewLevel();
                }
            }
            else
            {
                levelInstance.SetAsClosedLevel();
            }
        }

        public void SetLevelsCapacity(int capacity)
        {
            for (int i = _levelInstances.Count; i < capacity; i++)
            {
                var levelInstance = Instantiate(_levelPrefab, _levelsContainer);
                levelInstance.Initialize(i, OnLevelClicked);
                _levelInstances.Add(levelInstance);
            }

            for (int i = 0; i < capacity; i++)
            {
                _levelInstances[i].gameObject.SetActive(true);
            }

            for (int i = capacity; i < _levelInstances.Count; i++)
            {
                _levelInstances[i].gameObject.SetActive(false);
            }
        }

        private void OnLevelClicked(int levelIndex)
        {
            LevelClicked?.Invoke(levelIndex);
        }
    }
}