using System.Threading.Tasks;
using Dainty.UI.WindowBase;
using UnityEngine;

public class GameController : AWindowController<GameView>
{
    public override string WindowId { get; }
    private int _currentLevel = -1;
    private TextAsset _svgLevelAsset;

    public override void BeforeShow()
    {
        base.BeforeShow();


        var newLevel = LevelsManager.Instance.CurrentLevel;

        if (_currentLevel == newLevel)
        {
            //true - reset level colors to originals
            view.SetDefaults(true);
        }
        else
        {
            ApplicationController.Instance.SetActiveLoader(true);

            view.DestroyLevel();
            _svgLevelAsset = LevelsManager.Instance.CurrentLevelSvgTextAsset;

            view.InitLevel(_svgLevelAsset).ContinueWith(task =>
            {
                Debug.Log("after init level");
                ApplicationController.Instance.SetActiveLoader(false);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        _currentLevel = newLevel;
    }

    protected override void OnSubscribe()
    {
        view.OnLevelDone += ViewOnOnLevelDone;
    }
    protected override void OnUnSubscribe()
    {
        view.OnLevelDone -= ViewOnOnLevelDone;
    }

    private void ViewOnOnLevelDone(PassedLevelStats stats)
    {
        LevelsManager.Instance.SetPassedLevel(LevelsManager.Instance.CurrentLevel, stats.Percents);
        ApplicationController.Instance.UiManager.Open<LevelFinishedController, LevelFinishedSettings>(new LevelFinishedSettings() { Stats = stats });
    }

    public override void Dispose()
    {
        base.Dispose();
        view.DestroyLevel();
    }
}