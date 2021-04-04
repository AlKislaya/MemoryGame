using Dainty.UI.WindowBase;

public class LevelsSequenceController : AWindowController<LevelsSequenceView>
{
    public override string WindowId { get; }

    public override void BeforeShow()
    {
        var levelsAssetsCount = LevelsManager.Instance.LevelsAssetSequence.Levels.Count;
        var levelsSequence = LevelsManager.Instance.LevelsProgress.Levels;

        for (int i = 0; i < levelsSequence.Count; i++)
        {
            view.AddLevel(i, true, levelsSequence[i].PassedPercents);
        }

        for (int i = levelsSequence.Count; i < levelsAssetsCount; i++)
        {
            view.AddLevel(i, false);
        }
    }

    private void OnLevelClicked(int levelNumber)
    {
        LevelsManager.Instance.CurrentLevelNumber = levelNumber;
        //ApplicationController.Instance.UiManager.Open<GameController, GameWindowSettings>(new GameWindowSettings());
        ApplicationController.Instance.UiManager.Open<GameController>();
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
