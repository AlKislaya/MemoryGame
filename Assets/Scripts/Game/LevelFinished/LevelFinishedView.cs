using Dainty.UI.WindowBase;
using LocalizationModule;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelFinishedView : AWindowView
{
    public event Action OnMenuButtonClicked;
    public event Action OnPlayButtonClicked;
    public event Action OnReplayButtonClicked;
    public event Action RewardedAdButtonClicked;

    private const string ResultThree = "result_three";
    private const string ResultTwo = "result_two";
    private const string ResultOne = "result_one";
    private const string ResultZero = "result_zero";

    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _replayButton;
    [SerializeField] private GameObject _coinsLabelContainer;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private Button _rewardedAdButton;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private ParticleSystem _confettiParticle;
    [SerializeField] private List<StarController> _stars;
    private string _resultThree;
    private string _resultTwo;
    private string _resultOne;
    private string _resultZero;

    protected override void OnInitialized()
    {
        var windowSize = UiRoot.CanvasSize;
        _confettiParticle.transform.localPosition = new Vector3(windowSize.x / 2, windowSize.y, 1);

        SetLocals();
        Localization.Instance.OnLanguageChanged += SetLocals;
    }

    public void ShowAddedCoinsLabel(bool isShown, int coinsCount = 0)
    {
        if (coinsCount != 0)
        { 
            _coinsText.text = $"+ {coinsCount}"; 
        }
        _coinsLabelContainer.SetActive(isShown);
    }

    public void SetProgress(float passedPercents)
    {
        var activeCount = passedPercents == 0 ? 0 : passedPercents == 1 ? 3 : passedPercents < .5f ? 1 : 2;

        for (int i = 0; i < activeCount; i++)
        {
            _stars[i].SetActiveState(true);
        }
        for (int i = activeCount; i < _stars.Count; i++)
        {
            _stars[i].SetActiveState(false);
        }

        _resultText.text = activeCount == 0 ? _resultZero : activeCount == 1 ? _resultOne : activeCount == 2 ? _resultTwo : _resultThree;
    }

    public void SetActivePlayButton(bool isActive)
    {
        _playButton.gameObject.SetActive(isActive);
    }

    public void PlayConfettiAnimation()
    {
        _confettiParticle.Play();
    }
    public void StopConfettiAnimation()
    {
        _confettiParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    protected override void OnSubscribe()
    {
        _menuButton.onClick.AddListener(onMenuButtonClicked);
        _playButton.onClick.AddListener(onPlayButtonClicked);
        _replayButton.onClick.AddListener(onReplayButtonClicked);
        _rewardedAdButton.onClick.AddListener(OnRewardedAdButtonClicked);
    }

    protected override void OnUnSubscribe()
    {
        _menuButton.onClick.RemoveListener(onMenuButtonClicked);
        _playButton.onClick.RemoveListener(onPlayButtonClicked);
        _replayButton.onClick.RemoveListener(onReplayButtonClicked);
        _rewardedAdButton.onClick.RemoveAllListeners();
    }

    private void onReplayButtonClicked()
    {
        OnReplayButtonClicked?.Invoke();
    }

    private void onPlayButtonClicked()
    {
        OnPlayButtonClicked?.Invoke();
    }

    private void onMenuButtonClicked()
    {
        OnMenuButtonClicked?.Invoke();
    }

    private void OnRewardedAdButtonClicked()
    {
        RewardedAdButtonClicked?.Invoke();
    }

    private void SetLocals()
    {
        var localization = Localization.Instance;
        _resultZero = localization.GetLocalByKey(ResultZero);
        _resultOne = localization.GetLocalByKey(ResultOne);
        _resultTwo = localization.GetLocalByKey(ResultTwo);
        _resultThree = localization.GetLocalByKey(ResultThree);
    }
}
