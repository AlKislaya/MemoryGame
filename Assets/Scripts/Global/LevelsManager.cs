using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : Singleton<LevelsManager>
{
    public TextAsset CurrentLevelSvgTextAsset => _levelsSvgAssetsSequence.SvgLevelsAssets[CurrentLevel];

    public int CurrentLevel {
        get
        {
            return _currentLevel;
        }
        set
        {
            if (value >= _levelsSvgAssetsSequence.SvgLevelsAssets.Count)
            {
                Debug.LogError("Current level More than svg assets count! " + value);
                _currentLevel = _levelsSvgAssetsSequence.SvgLevelsAssets.Count - 1;
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

    public LevelsTextAssetsSequence LevelsSvgAssetsSequence => _levelsSvgAssetsSequence;
    public LevelsSequence LevelsSequence => _levelsSequence;

    [SerializeField] private LevelsTextAssetsSequence _levelsSvgAssetsSequence;
    private LevelsSequence _levelsSequence = new LevelsSequence();
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
                _levelsSequence = JsonUtility.FromJson<LevelsSequence>(levelsJson);
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
        _levelsSequence = new LevelsSequence
        {
            Levels = new List<Level>() { new Level() { IsPassed = false } }
        };
        SaveLevelsProgress();
    }

    public void SetPassedLevel(int levelIndex, float passedPercents)
    {
        var passedLevel = _levelsSequence.Levels[levelIndex];
        passedLevel.PassedPercents = passedPercents;
        passedLevel.IsPassed = true;
        if (levelIndex == _levelsSequence.Levels.Count - 1 && _levelsSequence.Levels.Count < _levelsSvgAssetsSequence.SvgLevelsAssets.Count)
        {
            _levelsSequence.Levels.Add(new Level(){ IsPassed = false });
        }

        SaveLevelsProgress();
    }

    private void SaveLevelsProgress()
    {
        Debug.Log("SAVED!     " + JsonUtility.ToJson(_levelsSequence));
        PlayerPrefs.SetString(PlayerPrefsKeys.LevelsSequenceKey, JsonUtility.ToJson(_levelsSequence));
    }
}