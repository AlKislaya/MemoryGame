using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameView : AWindowView
{
    private const int TimerSeconds = 2;

    public event Action<PassedLevelStats> OnLevelDone;
    public event Action<bool> OnBlockExitStateChanged;
    public event Action OnHintClicked;
    public bool BlockExit => _checkLevelAnimation != null && _checkLevelAnimation.IsPlaying();

    [SerializeField] private AnimatedButton _hintButton;
    [SerializeField] private PlayableObjectsController _playableController;
    [SerializeField] private Button _tapToStartButton;
    [SerializeField] private GameObject _tapToStartText;
    [SerializeField] private CounterElement _counterElement;
    [SerializeField] private GameObject _loader;

    [Header("Timer Colors")]
    [SerializeField] private Color _timerColor;
    [SerializeField] private Color _progressColor;
    [SerializeField] private Color _doneColor;

    private Sequence _startGameAnimation;
    private Sequence _placeObjectsAnimation;
    private Sequence _checkLevelAnimation;

    private string _seconds = "s";

    protected override void OnSubscribe()
    {
        base.OnSubscribe();
        _playableController.OnLevelEnded += OnLevelDoneInvoked;
        _hintButton.Button.onClick.AddListener(OnHintButtonClicked);
        _tapToStartButton.onClick.AddListener(StartGame);
    }

    protected override void OnUnSubscribe()
    {
        base.OnUnSubscribe();
        _playableController.OnLevelEnded -= OnLevelDoneInvoked;
        _hintButton.Button.onClick.RemoveAllListeners();
        _tapToStartButton.onClick.RemoveListener(StartGame);
    }

    public async Task InitLevel(Level levelAsset, CancellationToken token)
    {
        await _playableController.LoadLevel(levelAsset.SvgAsset, token);
    }

    public void ShowLoader(bool show)
    {
        _loader.SetActive(show);
    }

    //place objects animation
    public void PlaceObjects()
    {
        _placeObjectsAnimation = DOTween.Sequence();
        _placeObjectsAnimation.Append(_playableController.PlaceCard(.3f));
        _placeObjectsAnimation.AppendCallback(() =>
        {
            _tapToStartButton.gameObject.SetActive(true);
            _tapToStartText.SetActive(true);
        });

        _placeObjectsAnimation.Play();
    }

    //reset playable controller, reset colors, reset counter, show play btn zone, close swatches
    public void SetDefaults()
    {
        _playableController.SetDefaults();
        //hide tap to start buttons
        _tapToStartButton.gameObject.SetActive(false);
        _tapToStartText.SetActive(false);
        //init timer
        SetTimer();

        if (_hintButton.Button != null)
        {
            _hintButton.Button.interactable = false;
            _hintButton.SetAnimationEnabled(false);
        }
    }

    private void SetTimer()
    {
        _counterElement.SetDefaults();
        _counterElement.SetColor(_timerColor);
        _counterElement.SetText($"{TimerSeconds}{_seconds}");
        _counterElement.SetAmount(1);
    }

    public void ShowHint()
    {

    }

    public void DestroyLevel()
    {
        _playableController.DestroyLevel();
    }

    public void StopAnimations()
    {
        _startGameAnimation?.Kill();
        _placeObjectsAnimation?.Kill();
    }

    private void StartGame()
    {
        _tapToStartButton.gameObject.SetActive(false);
        _tapToStartText.SetActive(false);

        _playableController.OpenCard(true);
        _startGameAnimation = DOTween.Sequence();

        _startGameAnimation
            .Append(_counterElement.TimerTween(TimerSeconds, _seconds))
            .AppendCallback(() => _playableController.OpenCard(false))
            .AppendInterval(.3f)
            .AppendCallback(() =>
            {
                _playableController.StartGame();
                _hintButton.Button.interactable = true;
                _hintButton.SetAnimationEnabled(true);
            });

        _startGameAnimation.Play();
    }

    private void OnHintButtonClicked()
    {
        OnHintClicked?.Invoke();
    }

    private void OnLevelDoneInvoked(PassedLevelStats stats)
    {
        OnUnSubscribe();
        OnBlockExitStateChanged?.Invoke(true);
        _checkLevelAnimation = 
            _playableController.CheckCardAnimation()
            .AppendInterval(3f)
            .OnComplete(() =>
            {
                OnBlockExitStateChanged?.Invoke(false);
                OnLevelDone?.Invoke(stats);
            });
    }
}