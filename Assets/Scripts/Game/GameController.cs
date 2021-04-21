using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dainty.UI;
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
    private Task _loadingTask;
    private TopPanelController _topPanelController;
    private UiManager _uiManager;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        _uiManager = ApplicationController.Instance.UiManager;
        _topPanelController = ApplicationController.Instance.TopPanelController;
    }

    public void ReplayLevel()
    {
        view.SetDefaults();
        _waitingForLevelShow = true;
    }

    public void LoadLevel(string categoryKey, int levelIndex)
    {
        _categoryKey = categoryKey;
        _levelIndex = levelIndex;

        view.DestroyLevel();
        view.SetDefaults();
        view.ShowLoader(true);
        _waitingForLevelLoad = true;

        var levelAsset = LevelsManager.Instance.GetCategoryByKey(_categoryKey).LevelsSequence.Levels[_levelIndex];
        _loadingToken = new CancellationTokenSource();
        _loadingTask = view.InitLevel(levelAsset, _loadingToken.Token);
    }

    protected override void OnSubscribe()
    {
        view.OnLevelDone += ViewOnOnLevelDone;
        view.OnBlockExitStateChanged += OnBlockExitStateChanged;
        view.OnHintClicked += OnHintClicked;
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
            if (_loadingTask.Status != TaskStatus.RanToCompletion)
            {
                _loadingTask.ContinueWith(task =>
                {
                    Debug.Log("Task Done");
                    view.ShowLoader(false);
                    view.PlaceObjects();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                Debug.Log("Task Done Before animation ended");
                view.ShowLoader(false);
                view.PlaceObjects();
            }
        }
    }

    protected override void OnUnSubscribe()
    {
        view.OnLevelDone -= ViewOnOnLevelDone;
        view.OnBlockExitStateChanged -= OnBlockExitStateChanged;
        view.OnHintClicked -= OnHintClicked;
    }

    private void ViewOnOnLevelDone(PassedLevelStats stats)
    {
        _uiManager.Open<LevelFinishedController, LevelFinishedSettings>(new LevelFinishedSettings()
        {
            CategoryKey = _categoryKey,
            LevelIndex = _levelIndex,
            Stats = stats,
            GameController = this
        }, true);
    }

    private void OnHintClicked()
    {
        _uiManager.Open<AlertController, AlertSettings>(
            new AlertSettings()
            {
                DialogText = "Show hint?",
                Buttons = new List<AlertButtonSettings>() {
                                        new AlertButtonSettings() { Text = "i'm poor", Color = Color.grey, Callback = OnHintDeclined },
                                        new AlertButtonSettings() { Text = "give 10 bucks", Color = Color.green, Callback = OnHintApproved }}
            }, true);
    }

    private void OnHintDeclined()
    {
        _uiManager.Back();
    }

    private void OnHintApproved()
    {
        _uiManager.Back();
        view.ShowHint();
    }

    private void OnBlockExitStateChanged(bool isBlocked)
    {
        _topPanelController.IsBackButtonInteractable = !isBlocked;
    }

    protected override void OnEscape()
    {
        if (view.BlockExit)
        {
            return;
        }
        _uiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }

    public override void Dispose()
    {
        _loadingToken?.Cancel();
        view.StopAnimations();
        view.DestroyLevel();
        base.Dispose();
    }
}