using System.Threading;
using System.Threading.Tasks;
using Dainty.UI.WindowBase;
using UnityEngine;

public class GameController : AWindowController<GameView>
{
    public override string WindowId { get; }
    private bool _waitingForLevelLoad;
    private bool _waitingForLevelShow;
    private CancellationTokenSource _loadingToken;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        LoadLevel();
    }

    public void ReplayLevel()
    {
        view.SetDefaults();
        _waitingForLevelShow = true;
    }

    public void LoadLevel()
    {
        view.DestroyLevel(0);
        view.SetDefaults();
        view.ShowLoader(true);
        _waitingForLevelLoad = true;
    }

    protected override void OnSubscribe()
    {
        view.OnLevelDone += ViewOnOnLevelDone;


        if (_waitingForLevelShow)
        {
            _waitingForLevelShow = false;
            view.PlaceObjects();
        }

        if (_waitingForLevelLoad)
        {
            _waitingForLevelLoad = false;
            var levelAsset = LevelsManager.Instance.CurrentLevelAsset;
            _loadingToken = new CancellationTokenSource();
            view.InitLevel(levelAsset, _loadingToken.Token).ContinueWith(task =>
            {
                Debug.Log("Task Done");
                view.ShowLoader(false);
                view.PlaceObjects();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    protected override void OnUnSubscribe()
    {
        view.OnLevelDone -= ViewOnOnLevelDone;
    }

    private void ViewOnOnLevelDone(PassedLevelStats stats)
    {
        LevelsManager.Instance.SetPassedLevel(LevelsManager.Instance.CurrentLevelNumber, stats.Percents);
        ApplicationController.Instance.UiManager.Open<LevelFinishedController, LevelFinishedSettings>(new LevelFinishedSettings() { Stats = stats, GameController = this}, true);
    }

    public override void Dispose()
    {
        base.Dispose();
        _loadingToken.Cancel();
        view.StopAnimations();
        view.DestroyLevel(.3f);
    }
}