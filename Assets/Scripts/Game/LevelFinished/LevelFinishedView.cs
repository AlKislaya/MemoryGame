using Dainty.UI.WindowBase;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelFinishedView : AWindowView
{
    public event Action OnMenuButtonClicked;
    public event Action OnPlayButtonClicked;
    public event Action OnReplayButtonClicked;

    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _replayButton;
    [SerializeField] private List<StarController> _stars;

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
    }

    public void SetActivePlayButton(bool isActive)
    {
        _playButton.gameObject.SetActive(isActive);
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
