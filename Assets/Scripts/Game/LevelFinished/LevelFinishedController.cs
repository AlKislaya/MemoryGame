using Dainty.UI.WindowBase;

public class LevelFinishedSettings
{
    public PassedLevelStats Stats;
}

public class LevelFinishedController : AWindowController<LevelFinishedView>, IConfigurableWindow<LevelFinishedSettings>
{
    private const string Of = "of";
    private LevelsManager _levelsManager;
    public override string WindowId { get; }
    protected override void OnInitialize()
    {
        base.OnInitialize();

        _levelsManager = LevelsManager.Instance;

        view.SetActivePlayButton(_levelsManager.CurrentLevel != _levelsManager.LevelsSequence.Levels.Count - 1);
    }
    public void Initialize(LevelFinishedSettings levelFinishedSettings)
    {
        var stats = levelFinishedSettings.Stats;
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
        OpenGame();
    }

    private void ViewOnOnPlayButtonClicked()
    {
        _levelsManager.CurrentLevel++;
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
}
