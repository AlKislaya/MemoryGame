using Dainty.UI.WindowBase;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelFinishedView : AWindowView
{
    public event Action OnMenuButtonClicked;
    public event Action OnPlayButtonClicked;
    public event Action OnReplayButtonClicked;

    [SerializeField] private TextMeshProUGUI _percentsText;
    [SerializeField] private TextMeshProUGUI _rangeText;
    [SerializeField] private Image _progressFillAmountImage;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _replayButton;

    public void SetProgress(float percents, string range)
    {
        _percentsText.text = ((int)percents * 100).ToString();
        _rangeText.text = range;

        _progressFillAmountImage.fillAmount = percents;
    }

    protected override void OnSubscribe()
    {
        _menuButton.onClick.AddListener(onMenuButtonClicked);
        _playButton.onClick.AddListener(onPlayButtonClicked);
        _replayButton.onClick.AddListener(onReplayButtonClicked);
    }

    protected override void OnUnSubscribe()
    {
        _menuButton.onClick.RemoveListener(onMenuButtonClicked);
        _playButton.onClick.RemoveListener(onPlayButtonClicked);
        _replayButton.onClick.RemoveListener(onReplayButtonClicked);
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
}
