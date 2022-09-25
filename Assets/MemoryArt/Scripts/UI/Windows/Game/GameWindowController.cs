using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dainty.UI;
using Dainty.UI.WindowBase;
using MemoryArt.Game;
using MemoryArt.Global;
using UnityEngine;

namespace MemoryArt.UI.Windows
{
    public class GameWindowController : AWindowController<GameWindowView>
    {
        private const int SkipPrice = 10;
        private const string LevelHeaderKey = "level";
        private const string SkipLevelHeaderKey = "skip_level_header";
        private const string SkipLevelMoneyTextKey = "skip_level_money_text";
        private const string SkipLevelAdsTextKey = "skip_level_ads_text";
        private const string CancelKey = "cancel";
        private const string OkKey = "ok";

        public override string WindowId { get; }
        private string _categoryKey;
        private int _levelIndex;
        private bool _waitingForLevelLoad;
        private bool _waitingForLevelShow;
        private CancellationTokenSource _loadingToken;
        private Task _loadingTask;
        private TopPanelController _topPanelController;
        private string _levelHeader;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _topPanelController = ApplicationController.Instance.TopPanelController;
            _levelHeader = Localization.Instance.GetLocalByKey(LevelHeaderKey);
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

            view.SkipLevelAvailable = LevelsManager.Instance.GetLevelsProgressByCategory(categoryKey).Levels.Count ==
                                      levelIndex + 1;

            view.DestroyLevel();
            view.SetDefaults();
            view.ShowLoader(true);
            _waitingForLevelLoad = true;

            var levelAsset = LevelsManager.Instance.GetCategoryByKey(_categoryKey).LevelsSequence.Levels[_levelIndex];
            _loadingToken = new CancellationTokenSource();
            _loadingTask = view.InitLevel(levelAsset, _loadingToken.Token);

            ApplicationController.Instance.TopPanelController.Show($"{_levelHeader} {levelIndex + 1}");
        }

        protected override void OnSubscribe()
        {
            view.LevelDone += ViewOnLevelDone;
            view.BlockExitStateChanged += OnBlockExitStateChanged;
            view.SkipLevelClick += OnSkipLevelClick;

            ApplicationController.Instance.Camera.allowMSAA = true;
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
                        if (task.Exception != null)
                        {
                            Debug.LogException(task.Exception);
                            return;
                        }

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
            view.LevelDone -= ViewOnLevelDone;
            view.BlockExitStateChanged -= OnBlockExitStateChanged;
            view.SkipLevelClick -= OnSkipLevelClick;
        }

        private void ViewOnLevelDone(PassedLevelStats stats)
        {
            uiManager.Open<LevelFinishedWindowController, LevelFinishedWindowSettings>(new LevelFinishedWindowSettings
            {
                CategoryKey = _categoryKey,
                LevelIndex = _levelIndex,
                Stats = stats,
                GameWindowController = this
            }, true);
        }

        private void OnSkipLevelClick()
        {
            var localization = Localization.Instance;
            if (MoneyController.Instance.MoneyBalance < SkipPrice)
            {
                //offer ads
                uiManager.Open<AlertWindowController, AlertWindowSettings>(
                    new AlertWindowSettings
                    {
                        HeaderText = localization.GetLocalByKey(SkipLevelHeaderKey),
                        DialogText = localization.GetLocalByKey(SkipLevelAdsTextKey),
                        OnBackButtonClicked = OnSkipLevelDeclined,
                        Buttons = new List<AlertButtonSettings>
                        {
                            new AlertButtonSettings
                            {
                                Text = localization.GetLocalByKey(CancelKey), Color = AlertButtonColor.White,
                                Callback = OnSkipLevelDeclined
                            },
                            new AlertButtonSettings
                            {
                                Text = localization.GetLocalByKey(OkKey),
                                Color = AlertButtonColor.Green,
                                Callback = OnSkipLevelWatchAdsApproved
                            }
                        }
                    }, true);
            }
            else
            {
                uiManager.Open<AlertWindowController, AlertWindowSettings>(
                    new AlertWindowSettings
                    {
                        HeaderText = localization.GetLocalByKey(SkipLevelHeaderKey),
                        DialogText = localization.GetLocalByKey(SkipLevelMoneyTextKey),
                        OnBackButtonClicked = OnSkipLevelDeclined,
                        Buttons = new List<AlertButtonSettings>
                        {
                            new AlertButtonSettings
                            {
                                Text = localization.GetLocalByKey(CancelKey), Color = AlertButtonColor.White,
                                Callback = OnSkipLevelDeclined
                            },
                            new AlertButtonSettings
                            {
                                Text = localization.GetLocalByKey(OkKey),
                                Color = AlertButtonColor.Green,
                                Callback = SkipLevel
                            }
                        }
                    }, true);
            }
        }

        private void OnSkipLevelDeclined()
        {
            uiManager.Back();
        }

        private void OnSkipLevelWatchAdsApproved()
        {
            uiManager.Back();
            ViewOnLevelDone(new PassedLevelStats
            {
                SelectableCount = view.RoundsCount,
                RightSelectablesCount = view.RoundsCount - 1
            });
        }

        private void SkipLevel()
        {
            uiManager.Back();
            if (MoneyController.Instance.GetMoney(SkipPrice))
            {
                ViewOnLevelDone(new PassedLevelStats
                {
                    SelectableCount = view.RoundsCount,
                    RightSelectablesCount = view.RoundsCount - 1
                });
            }
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

            uiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
        }

        public override void Dispose()
        {
            _loadingToken?.Cancel();
            view.StopAnimations();
            view.DestroyLevel();
            ApplicationController.Instance.Camera.allowMSAA = false;
            base.Dispose();
        }
    }
}