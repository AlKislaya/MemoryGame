using System.Threading;
using System.Threading.Tasks;
using Dainty.UI.WindowBase;
using UnityEngine;

public class GameController : AWindowController<GameView>
{
    public override string WindowId { get; }
    private string _categoryKey;
    private int _levelIndex;
    private bool _waitingForLevelLoad;
    private bool _waitingForLevelShow;
    private CancellationTokenSource _loadingToken;

    public void ReplayLevel()
    {
        view.SetDefaults();
        _waitingForLevelShow = true;
    }

    public void LoadLevel(string categoryKey, int levelIndex)
    {
        _categoryKey = categoryKey;
        _levelIndex = levelIndex;
        Debug.Log("Game: "+_categoryKey+" "+levelIndex);

        view.DestroyLevel();
        view.SetDefaults();
        view.ShowLoader(true);
        _waitingForLevelLoad = true;
    }

    protected override void OnSubscribe()
    {
        view.OnLevelDone += ViewOnOnLevelDone;
        ProcessLevel();
    }

    private void ProcessLevel()
    {
        if (_waitingForLevelShow)
        {
            _waitingForLevelShow = false;
            view.PlaceObjects();
        }

        if (_waitingForLevelLoad)
        {
            _waitingForLevelLoad = false;
            var levelAsset = LevelsManager.Instance.GetCategoryByKey(_categoryKey).LevelsSequence.Levels[_levelIndex];
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
        ApplicationController.Instance.UiManager.Open<LevelFinishedController, LevelFinishedSettings>(new LevelFinishedSettings()
        {
            CategoryKey = _categoryKey,
            LevelIndex = _levelIndex,
            Stats = stats,
            GameController = this
        }, true);
    }

    public override void Dispose()
    {
        base.Dispose();
        _loadingToken?.Cancel();
        view.StopAnimations();
        view.DestroyLevel();
    }
}