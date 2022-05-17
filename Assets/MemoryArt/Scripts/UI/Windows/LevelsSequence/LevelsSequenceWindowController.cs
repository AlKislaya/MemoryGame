using System.Collections.Generic;
using Dainty.UI;
using Dainty.UI.WindowBase;
using UnityEngine;
using LocalizationModule;

public class LevelsSequenceWindowSettings
{
    public LevelsCategory Category;
}

public class LevelsSequenceWindowController : AWindowController<LevelsSequenceWindowView>, IConfigurableWindow<LevelsSequenceWindowSettings>
{
    private const string HeaderKey = "levels_sequence";
    public override string WindowId { get; }

    private string _categoryKey;
    private LevelsCategory _levelsCategory;
    private List<LevelProgress> _levelsProgress;
    private string _header;

    public void Initialize(LevelsSequenceWindowSettings settings)
    {
        _header = Localization.Instance.GetLocalByKey(HeaderKey);

        _categoryKey = settings.Category.Key;
        _levelsCategory = settings.Category;

        if (_levelsCategory == null)
        {
            Debug.LogError("Did not find category "+ _categoryKey);
            uiManager.Back();
            return;
        }
    }

    public override void BeforeShow()
    {
        ApplicationController.Instance.TopPanelController.Show(_header);

        if (_levelsCategory == null)
        {
            Debug.LogError("Didn't init category");
            uiManager.Back();
            return;
        }

        var levelsManager = LevelsManager.Instance;
        _levelsProgress = levelsManager.GetLevelsProgressByCategory(_categoryKey).Levels;
        var levels = _levelsCategory.LevelsSequence.Levels;
        var levelsCapacity = _levelsCategory.LevelsSequence.Levels.Count;

        view.SetLevelsCapacity(levelsCapacity);

        //checking that new level exists
        if (_levelsProgress[_levelsProgress.Count - 1].IsPassed 
            && _levelsProgress[_levelsProgress.Count - 1].PassedPercents != 0 
            && _levelsProgress.Count != levelsCapacity)
        {
            levelsManager.SetNewLevelProgress(_categoryKey);
            _levelsProgress = levelsManager.GetLevelsProgressByCategory(_categoryKey).Levels;
        }

        for (int i = 0; i < _levelsProgress.Count; i++)
        {
            view.AddLevel(i, true, _levelsProgress[i], levels[i].Preview);
        }

        for (int i = _levelsProgress.Count; i < levelsCapacity; i++)
        {
            view.AddLevel(i, false);
        }
    }

    private void OnLevelClicked(int levelNumber)
    {
        var gameController =
            uiManager.Open<GameWindowController>(WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
        gameController.LoadLevel(_categoryKey, levelNumber);
    }

    protected override void OnSubscribe()
    {
        view.OnLevelClicked += OnLevelClicked;
    }

    protected override void OnUnSubscribe()
    {
        view.OnLevelClicked -= OnLevelClicked;
    }

    protected override void OnEscape()
    {
        uiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }
}
