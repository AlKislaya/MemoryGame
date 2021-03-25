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

    public void AddLevel(int levelIndex, bool isOpened, float passedPercents = 0)
    {
        if (_levelInstances.Count - 1 >= levelIndex)
        {
            var levelInstance = _levelInstances[levelIndex];

            if (isOpened)
            {
                if (levelInstance.IsOpened)
                {
                    levelInstance.UpdateLevel(passedPercents);
                }
                else
                {
                    levelInstance.SetAsOpenedLevel(passedPercents);
                }
            }
            else if (levelInstance.IsOpened)
            {
                levelInstance.SetAsLockedLevel();
            }

            return;
        }

        var newLevelInstance = Instantiate(_levelPrefab, _levelsContainer);
        newLevelInstance.Initialize(levelIndex, onLevelClicked);

        if (isOpened)
        {
            newLevelInstance.SetAsOpenedLevel(passedPercents);
        }
        else
        {
            newLevelInstance.SetAsLockedLevel();
        }

        _levelInstances.Add(newLevelInstance);
    }

    private void onLevelClicked(int levelIndex)
    {
        OnLevelClicked?.Invoke(levelIndex);
    }
}