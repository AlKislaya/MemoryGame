using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : Singleton<LevelsManager>
{
    public Level CurrentLevelAsset => _levelsAssetSequence.Levels[CurrentLevelNumber];

    public int CurrentLevelNumber {
        get
        {
            return _currentLevel;
        }
        set
        {
            if (value >= _levelsAssetSequence.Levels.Count)
            {
                Debug.LogError("Current level More than svg assets count! " + value);
                _currentLevel = _levelsAssetSequence.Levels.Count - 1;
            }
            else if (value < 0)
            {
                Debug.LogError("Current level less than 0! " + value);
                _currentLevel = 0;
            }
            else
            {
                _currentLevel = value;
            }
        }
    }

    public LevelsSequence LevelsAssetSequence => _levelsAssetSequence;
    public LevelsProgress LevelsProgress => _levelsProgress;

    [SerializeField] private LevelsSequence _levelsAssetSequence;
    private LevelsProgress _levelsProgress = new LevelsProgress();
    private int _currentLevel;

    protected override void Awake()
    {
        base.Awake();
        //init levels
        var levelsJson = PlayerPrefs.GetString(PlayerPrefsKeys.LevelsSequenceKey);
        if (string.IsNullOrEmpty(levelsJson))
        {
            InitLevelSequence();
        }
        else
        {
            try
            {
                _levelsProgress = JsonUtility.FromJson<LevelsProgress>(levelsJson);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                InitLevelSequence();
            }
        }
    }

    private void InitLevelSequence()
    {
        _levelsProgress = new LevelsProgress
        {
            Levels = new List<LevelProgress>() { new LevelProgress() { IsPassed = false } }
        };
        SaveLevelsProgress();
    }

    public void SetPassedLevel(int levelIndex, float passedPercents)
    {
        var passedLevel = _levelsProgress.Levels[levelIndex];
        passedLevel.PassedPercents = passedPercents;
        passedLevel.IsPassed = true;
        if (levelIndex == _levelsProgress.Levels.Count - 1 && _levelsProgress.Levels.Count < _levelsAssetSequence.Levels.Count)
        {
            _levelsProgress.Levels.Add(new LevelProgress(){ IsPassed = false });
        }

        SaveLevelsProgress();
    }

    private void SaveLevelsProgress()
    {
        Debug.Log("SAVED!     " + JsonUtility.ToJson(_levelsProgress));
        PlayerPrefs.SetString(PlayerPrefsKeys.LevelsSequenceKey, JsonUtility.ToJson(_levelsProgress));
    }
}