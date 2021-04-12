using System.Collections.Generic;
using Dainty.UI.WindowBase;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MainView : AWindowView
{
    [Serializable]
    public class LevelType
    {
        public string Key;
        public Button Button;
        public RectTransform RectTransform;
    }

    public Action OnSettingsClicked;
    public Action<string> OnLevelsTypeClicked;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _typesLeftButton;
    [SerializeField] private Button _typesRightButton;
    [SerializeField] private List<LevelType> _levelTypes;
    private int _activeTypeIndex = 0;
    private float _screenWidth;
    private Ease _ease = Ease.OutSine;
    private float _duration = .3f;

    private void Awake()
    {
        _screenWidth = ApplicationController.Instance.UiManager.UiRoot.CanvasSize.x;
        for (int i = 0; i < _levelTypes.Count; i++)
        {
            if (i < _activeTypeIndex)
            {
                _levelTypes[i].RectTransform.anchoredPosition = new Vector2(-_screenWidth, 0);
            }
            else if (i > _activeTypeIndex)
            {
                _levelTypes[i].RectTransform.anchoredPosition = new Vector2(_screenWidth, 0);
            }
            else
            {
                _levelTypes[i].RectTransform.anchoredPosition = Vector2.zero;
            }
        }
        _typesLeftButton.onClick.AddListener(() => OnLevelsTypesSwitched(1));
        _typesRightButton.onClick.AddListener(() => OnLevelsTypesSwitched(-1));

        _levelTypes.ForEach(x => x.Button.onClick.AddListener(() => OnLevelsTypeClicked?.Invoke(x.Key)));
        _settingsButton.onClick.AddListener(onSettingsClicked);
    }

    private void OnLevelsTypesSwitched(int direction)
    {
        SetEnabledTypesSwitchButtons(false);
        ChangeActiveType(direction, () => SetEnabledTypesSwitchButtons(true));
    }

    private void SetEnabledTypesSwitchButtons(bool isActive)
    {
        _typesLeftButton.enabled = isActive;
        _typesRightButton.enabled = isActive;
    }

    private void ChangeActiveType(int direction, Action onAnimationCompleted)
    {
        var newIndex = _activeTypeIndex;
        newIndex += direction;
        if (newIndex < 0)
        {
            newIndex = _levelTypes.Count - 1;
        }
        else if (newIndex >= _levelTypes.Count)
        {
            newIndex = 0;
        }

        _levelTypes[newIndex].RectTransform.anchoredPosition = new Vector2(direction == -1 ? _screenWidth : -_screenWidth, 0);
        DOTween.Sequence()
            .Append(_levelTypes[_activeTypeIndex].RectTransform.DOAnchorPos(new Vector2(direction == 1 ? _screenWidth : -_screenWidth, 0), _duration).SetEase(_ease))
            .Join(_levelTypes[newIndex].RectTransform.DOAnchorPos(Vector2.zero, _duration).SetEase(_ease))
            .AppendCallback(() => onAnimationCompleted?.Invoke());
        _activeTypeIndex = newIndex;
    }

    private void onSettingsClicked()
    {
        OnSettingsClicked?.Invoke();
    }
}
