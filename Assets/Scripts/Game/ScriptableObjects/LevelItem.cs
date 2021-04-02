using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    public bool IsOpened => _button.enabled;

    [SerializeField] private Button _button;
    [SerializeField] private Image _percentsImage;
    [SerializeField] private TextMeshProUGUI _levelNumberText;
    [SerializeField] private GameObject _lockImage;

    private int _levelNumber;

    public void Initialize(int levelNumber, Action<int> onButtonClickedCallback)
    {
        _levelNumber = levelNumber;
        _levelNumberText.text = (levelNumber + 1).ToString();

        _button.onClick.AddListener(() => onButtonClickedCallback(_levelNumber));
    }

    public void SetAsOpenedLevel(float passedPercent)
    {
        UpdateLevel(passedPercent);
        SwitchState(true);
    }

    public void UpdateLevel(float passedPercent)
    {
        _percentsImage.fillAmount = passedPercent;
    }

    public void SetAsLockedLevel()
    {
        SwitchState(false);
    }

    private void SwitchState(bool isOpen)
    {
        _button.enabled = isOpen;
        _percentsImage.gameObject.SetActive(isOpen);
        _levelNumberText.gameObject.SetActive(isOpen);
        _lockImage.SetActive(!isOpen);
    }
}
