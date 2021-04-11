using System.Collections.Generic;
using Dainty.UI;
using Dainty.UI.WindowBase;
using UnityEngine;

public class LevelsSequenceSettings
{
    public LevelsCategory Category;
}

public class LevelsSequenceController : AWindowController<LevelsSequenceView>, IConfigurableWindow<LevelsSequenceSettings>
{
    private const string Header = "Levels";
    public override string WindowId { get; }

    private string _categoryKey;
    private LevelsCategory _levelsCategory;
    private List<LevelProgress> _levelsProgress;

    public void Initialize(LevelsSequenceSettings settings)
    {
        _categoryKey = settings.Category.Key;
        _levelsCategory = settings.Category;

        if (_levelsCategory == null)
        {
            Debug.LogError("Did not find category "+ _categoryKey);
            ApplicationController.Instance.UiManager.Back();
            return;
        }
    }

    public override void BeforeShow()
    {
        ApplicationController.Instance.TopPanelController.Show(Header);

        if (_levelsCategory == null)
        {
            Debug.LogError("Didn't init category");
            ApplicationController.Instance.UiManager.Back();
            return;
        }

        _levelsProgress = LevelsManager.Instance.GetLevelsProgressByCategory(_categoryKey).Levels;
        var levels = _levelsCategory.LevelsSequence.Levels;
        var levelsCapacity = _levelsCategory.LevelsSequence.Levels.Count;

        view.SetLevelsCapacity(levelsCapacity);

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
        var gameController = ApplicationController.Instance.UiManager
            .Open<GameController>(WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
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
}
