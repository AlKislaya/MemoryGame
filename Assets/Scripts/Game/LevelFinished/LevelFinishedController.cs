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
    private LevelsManager _levelsManager;
    private GameController _gameController;
    private LevelFinishedSettings _levelFinishedSettings;
    public override string WindowId { get; }
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
        var stats = levelFinishedSettings.Stats;
        var categoryLevelsCount = _levelsManager.GetCategoryByKey(levelFinishedSettings.CategoryKey).LevelsSequence.Levels.Count;
        var levelsProgress = _levelsManager.GetLevelsProgressByCategory(levelFinishedSettings.CategoryKey).Levels;
        
        if (stats.Percents > levelsProgress[levelIndex].PassedPercents)
        {
            _levelsManager.SetPassedLevel(levelFinishedSettings.CategoryKey, levelIndex, stats.Percents);
            if (levelsProgress.Count - 1 == levelIndex
                && levelIndex + 1 < categoryLevelsCount)
            {
                _levelsManager.SetNewLevelProgress(levelFinishedSettings.CategoryKey);
            }
        }
        else if (!levelsProgress[levelIndex].IsPassed && stats.Percents == 0)
        {
            _levelsManager.SetPassedLevel(levelFinishedSettings.CategoryKey, levelIndex, 0);
        }


        view.SetProgress(stats.Percents);
        view.SetActivePlayButton(!((stats.Percents == 0 && levelsProgress.Count - 1 == levelIndex)
                                   || levelFinishedSettings.LevelIndex == categoryLevelsCount - 1));
    }

    public override void BeforeShow()
    {
        base.BeforeShow();
        view.PlayConfettiAnimation();
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
        var manager = ApplicationController.Instance.UiManager;
        manager.Back();
        manager.Back();
    }

    private void OpenGame()
    {
        ApplicationController.Instance.UiManager.Back();
    }

    protected override void OnEscape()
    {
        base.OnEscape();
        ApplicationController.Instance.UiManager.Back();
    }
}
