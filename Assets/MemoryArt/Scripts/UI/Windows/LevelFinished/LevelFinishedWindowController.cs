using Dainty.UI;
using Dainty.UI.WindowBase;

public class LevelFinishedWindowSettings
{
    public string CategoryKey;
    public int LevelIndex;
    public PassedLevelStats Stats;
    public GameWindowController GameWindowController;
}

public class LevelFinishedWindowController : AWindowController<LevelFinishedWindowView>, IConfigurableWindow<LevelFinishedWindowSettings>
{
    public override string WindowId { get; }

    private const int CoinsValue = 10;

    private LevelsManager _levelsManager;
    private GameWindowController _gameWindowController;
    private LevelFinishedWindowSettings _settings;
    private float _currentPercents;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        _levelsManager = LevelsManager.Instance;
    }

    public void Initialize(LevelFinishedWindowSettings data)
    {
        _gameWindowController = data.GameWindowController;
        _settings = data;
        var levelIndex = data.LevelIndex;
        _currentPercents = (float)data.Stats.RightSelectablesCount / (float)data.Stats.SelectableCount;
        var categoryLevelsCount = _levelsManager.GetCategoryByKey(data.CategoryKey).LevelsSequence.Levels.Count;
        var levelsProgress = _levelsManager.GetLevelsProgressByCategory(data.CategoryKey).Levels;
        view.ShowAddedCoinsLabel(false);
        
        if (_currentPercents > levelsProgress[levelIndex].PassedPercents)
        {
            _levelsManager.SetPassedLevel(data.CategoryKey, levelIndex, _currentPercents);
            if (levelsProgress.Count - 1 == levelIndex
                && levelIndex + 1 < categoryLevelsCount)
            {
                _levelsManager.SetNewLevelProgress(data.CategoryKey);
            }
            if (_currentPercents >= 1f)
            {
                MoneyController.Instance.AddMoney(CoinsValue);
                view.ShowAddedCoinsLabel(true, CoinsValue);
            }
        }
        else if (!levelsProgress[levelIndex].IsPassed && _currentPercents == 0)
        {
            _levelsManager.SetPassedLevel(data.CategoryKey, levelIndex, 0);
        }


        view.SetProgress(_currentPercents);
        view.SetActivePlayButton(!((_currentPercents == 0 && levelsProgress.Count - 1 == levelIndex)
                                   || data.LevelIndex == categoryLevelsCount - 1));
    }

    public override void BeforeShow()
    {
        base.BeforeShow();
        if (_currentPercents >= .5f)
        {
            view.PlayConfettiAnimation();
        }
    }

    protected override void OnSubscribe()
    {
        view.OnMenuButtonClicked += ViewOnOnMenuButtonClicked;
        view.OnPlayButtonClicked += ViewOnOnPlayButtonClicked;
        view.OnReplayButtonClicked += ViewOnOnReplayButtonClicked;
    }
    protected override void OnUnSubscribe()
    {
        view.OnMenuButtonClicked -= ViewOnOnMenuButtonClicked;
        view.OnPlayButtonClicked -= ViewOnOnPlayButtonClicked;
        view.OnReplayButtonClicked -= ViewOnOnReplayButtonClicked;
        view.StopConfettiAnimation();
    }

    private void ViewOnOnReplayButtonClicked()
    {
        _gameWindowController.ReplayLevel();
        OpenGame();
    }

    private void ViewOnOnPlayButtonClicked()
    {
        _gameWindowController.LoadLevel(_settings.CategoryKey, _settings.LevelIndex + 1);
        OpenGame();
    }

    private void ViewOnOnMenuButtonClicked()
    {
        uiManager.Back();
        uiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }

    private void OpenGame()
    {
        uiManager.Back();
    }

    protected override void OnEscape()
    {
        base.OnEscape();
        uiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }
}
