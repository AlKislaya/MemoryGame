using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    public bool IsOpened => _isOpened;

    [SerializeField] private Button _button;
    [SerializeField] private ButtonAnimation _buttonAnimation;
    [SerializeField] private Image _previewImage;
    [SerializeField] private Image _fillMainRectImage;
    [SerializeField] private TextMeshProUGUI _levelNumberText;
    [SerializeField] private GameObject _newLevelPanel;
    [SerializeField] private Sprite _questionSprite;
    [SerializeField] private Sprite _lockSprite;
    [SerializeField] private Color _lockModeFillcolor;
    [SerializeField] private List<GameObject> _openedLevelItems;
    private int _levelNumber;
    private bool _isOpened;
    private bool _isNew;

    public void Initialize(int levelNumber, Action<int> onButtonClickedCallback)
    {
        _levelNumberText.text = (levelNumber + 1).ToString();
        _levelNumber = levelNumber;
        _button.onClick.AddListener(() => onButtonClickedCallback(_levelNumber));
    }

    public void SetAsOpenedLevel(Sprite preview, float passedPercents)
    {
        _previewImage.sprite = preview;
        SetEnable(true);
    }

    public void SetAsClosedLevel()
    {
        SetEnable(false);
    }

    public void SetAsNewLevel()
    {
        SetEnable(true, true);
    }

    private void SetEnable(bool isEnabled, bool isNew = false)
    {
        if (isEnabled == _isOpened && _isNew == isNew)
        {
            return;
        }

        _isOpened = isEnabled;
        _isNew = isNew;

        _button.enabled = isEnabled;
        _buttonAnimation.SetAnimationEnabled(isEnabled);
        if (!isEnabled)
        {
            _previewImage.sprite = _lockSprite;
        }
        else if (isNew)
        {
            _previewImage.sprite = _questionSprite;
        }

        _fillMainRectImage.color = isEnabled ? Color.white : _lockModeFillcolor;
        _openedLevelItems.ForEach(x => x.SetActive(!isNew && isEnabled));
        _newLevelPanel.SetActive(isNew);
    }
}