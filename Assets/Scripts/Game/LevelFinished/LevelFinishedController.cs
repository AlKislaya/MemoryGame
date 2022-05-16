using Dainty.UI;
using Dainty.UI.WindowBase;

public class LevelFinishedSettings
{
    public string CategoryKey;
    public int LevelIndex;
    public PassedLevelStats Stats;
    public GameController GameController;
}

public class LevelFinishedController : AWindowController<LevelFinishedView>, IConfigurableWindow<LevelFinishedSettings>
{
    public override string WindowId { get; }

    private const int CoinsValue = 10;

    private LevelsManager _levelsManager;
    private GameController _gameController;
    private LevelFinishedSettings _levelFinishedSettings;
    private float _currentPercents;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        _levelsManager = LevelsManager.Instance;
    }

    public void Initialize(LevelFinishedSettings levelFinishedSettings)
    {
        _gameController = levelFinishedSettings.GameController;
        _levelFinishedSettings = levelFinishedSettings;
        var levelIndex = levelFinishedSettings.LevelIndex;
        _currentPercents = (float)levelFinishedSettings.Stats.RightSelectablesCount / (float)levelFinishedSettings.Stats.SelectableCount;
        var categoryLevelsCount = _levelsManager.GetCategoryByKey(levelFinishedSettings.CategoryKey).LevelsSequence.Levels.Count;
        var levelsProgress = _levelsManager.GetLevelsProgressByCategory(levelFinishedSettings.CategoryKey).Levels;
        view.ShowAddedCoinsLabel(false);
        
        if (_currentPercents > levelsProgress[levelIndex].PassedPercents)
        {
            _levelsManager.SetPassedLevel(levelFinishedSettings.CategoryKey, levelIndex, _currentPercents);
            if (levelsProgress.Count - 1 == levelIndex
                && levelIndex + 1 < categoryLevelsCount)
            {
                _levelsManager.SetNewLevelProgress(levelFinishedSettings.CategoryKey);
            }
            if (_currentPercents >= 1f)
            {
                MoneyController.Instance.AddMoney(CoinsValue);
                view.ShowAddedCoinsLabel(true, CoinsValue);
            }
        }
        else if (!levelsProgress[levelIndex].IsPassed && _currentPercents == 0)
        {
            _levelsManager.SetPassedLevel(levelFinishedSettings.CategoryKey, levelIndex, 0);
        }


        view.SetProgress(_currentPercents);
        view.SetActivePlayButton(!((_currentPercents == 0 && levelsProgress.Count - 1 == levelIndex)
                                   || levelFinishedSettings.LevelIndex == categoryLevelsCount - 1));
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
        view.RewardedAdButtonClicked += ViewOnRewardedAdButtonClicked;
    }
    protected override void OnUnSubscribe()
    {
        view.OnMenuButtonClicked -= ViewOnOnMenuButtonClicked;
        view.OnPlayButtonClicked -= ViewOnOnPlayButtonClicked;
        view.OnReplayButtonClicked -= ViewOnOnReplayButtonClicked;
        view.RewardedAdButtonClicked -= ViewOnRewardedAdButtonClicked;

        view.StopConfettiAnimation();
    }

    private void ViewOnOnReplayButtonClicked()
    {
        _gameController.ReplayLevel();
        OpenGame();
    }

    private void ViewOnOnPlayButtonClicked()
    {
        _gameController.LoadLevel(_levelFinishedSettings.CategoryKey, _levelFinishedSettings.LevelIndex + 1);
        OpenGame();
    }

    private void ViewOnOnMenuButtonClicked()
    {
        uiManager.Back();
        uiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }

    private void ViewOnRewardedAdButtonClicked()
    {
        ApplicationController.Instance.AdsController.ShowRewarded(null, success =>
        {
            if (success)
            {
                MoneyController.Instance.AddMoney(20);
            }
        }, null);
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
