using System;
using System.Threading.Tasks;
using Dainty.UI.WindowBase;
using UnityEngine;

public class GameController : AWindowController<GameView>
{
    public override string WindowId { get; }
    private int _currentLevel = -1;

    public override void BeforeShow()
    {
        base.BeforeShow();

        var newLevelNumber = LevelsManager.Instance.CurrentLevel;

        if (_currentLevel == newLevelNumber)
        {
            //if level is already loaded
            //true - reset level colors to originals
            view.SetDefaults(true);
        }
        else
        {
            ApplicationController.Instance.SetActiveLoader(true);

            //destroy sprites container
            view.DestroyLevel();

            var levelAsset = LevelsManager.Instance.CurrentLevelAsset;

            view.InitLevel(levelAsset).ContinueWith(task =>
            {
                ApplicationController.Instance.SetActiveLoader(false);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        _currentLevel = newLevelNumber;
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