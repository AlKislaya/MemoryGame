using Dainty.UI.WindowBase;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertWindowView : AWindowView
{
    [Serializable]
    public class ButtonColors
    {
        public AlertButtonColor AlertColor;
        public Color BackgroundColor;
        public Color TextColor;
    }

    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private Transform _buttonsContainer;
    [SerializeField] private Button _backgroundButton;
    [SerializeField] private AlertButton _alertButtonPrefab;
    [SerializeField] private List<ButtonColors> _buttonColors;

    private readonly List<AlertButton> _alertButtons = new List<AlertButton>();

    public void InitializeAlert(AlertWindowSettings data)
    {
        _headerText.text = data.HeaderText;
        _dialogText.text = data.DialogText;

        if (data.OnBackButtonClicked != null)
        {
            _backgroundButton.onClick.AddListener(data.OnBackButtonClicked);
        }

        for (int i = 0; i < data.Buttons.Count; i++)
        {
            if (_alertButtons.Count <= i)
            {
                var newButton = Instantiate(_alertButtonPrefab, _buttonsContainer);
                _alertButtons.Add(newButton);
            }

            var colors = _buttonColors.FirstOrDefault(x => x.AlertColor == data.Buttons[i].Color);
            _alertButtons[i].Initialize(data.Buttons[i], colors.BackgroundColor, colors.TextColor);
            _alertButtons[i].SetActive(true);
        }

        for (int i = data.Buttons.Count; i < _alertButtons.Count; i++)
        {
            _alertButtons[i].SetActive(false);
        }
    }

    protected override void OnUnSubscribe()
    {
        _backgroundButton.onClick.RemoveAllListeners();
        _alertButtons.ForEach(x => x.RemoveListeners());
    }
}