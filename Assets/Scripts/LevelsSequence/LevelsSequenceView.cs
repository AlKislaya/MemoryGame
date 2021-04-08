using System;
using System.Collections.Generic;
using Dainty.UI.WindowBase;
using UnityEngine;

public class LevelsSequenceView : AWindowView
{
    [SerializeField] private LevelItem _levelPrefab;
    [SerializeField] private Transform _levelsContainer;
    private List<LevelItem> _levelInstances = new List<LevelItem>();
    public event Action<int> OnLevelClicked;

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
            levelInstance.Initialize(levelIndex, onLevelClicked);
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
            levelInstance.Initialize(i, onLevelClicked);
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

    private void onLevelClicked(int levelIndex)
    {
        OnLevelClicked?.Invoke(levelIndex);
    }
}