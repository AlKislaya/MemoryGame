using Dainty.UI.WindowBase;

public class LevelFinishedSettings
{
    public PassedLevelStats Stats;
    public GameController GameController;
}

public class LevelFinishedController : AWindowController<LevelFinishedView>, IConfigurableWindow<LevelFinishedSettings>
{
    private const string Of = "of";
    private LevelsManager _levelsManager;
    private GameController _gameController;
    public override string WindowId { get; }
    protected override void OnInitialize()
    {
        base.OnInitialize();

        _levelsManager = LevelsManager.Instance;

        view.SetActivePlayButton(_levelsManager.CurrentLevelNumber != _levelsManager.LevelsAssetSequence.Levels.Count - 1);
    }
    public void Initialize(LevelFinishedSettings levelFinishedSettings)
    {
        var stats = levelFinishedSettings.Stats;
        _gameController = levelFinishedSettings.GameController;
        view.SetProgress(stats.Percents, $"{stats.RightPaintablesCount} {Of} {stats.PaintablesCount}");
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
    }

    private void ViewOnOnReplayButtonClicked()
    {
        _gameController.ReplayLevel();
        OpenGame();
    }

    private void ViewOnOnPlayButtonClicked()
    {
        _levelsManager.CurrentLevelNumber++;
        _gameController.LoadLevel();
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
