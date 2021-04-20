using Dainty.UI.WindowBase;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertView : AWindowView
{
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private Transform _buttonsContainer;
    [SerializeField] private AlertButton _alertButtonPrefab;
    private List<AlertButton> _alertButtons = new List<AlertButton>();

    public void InitializeAlert(AlertSettings data)
    {
        _headerText.text = data.HeaderText;
        _dialogText.text = data.DialogText;

        for (int i = 0; i < data.Buttons.Count; i++)
        {
            if (_alertButtons.Count <= i)
            {
                var newButton = Instantiate(_alertButtonPrefab, _buttonsContainer);
                _alertButtons.Add(newButton);
            }
            _alertButtons[i].Initialize(data.Buttons[i]);
            _alertButtons[i].SetActive(true);
        }

        for (int i = data.Buttons.Count; i < _alertButtons.Count; i++)
        {
            _alertButtons[i].SetActive(false);
        }
    }

    protected override void OnUnSubscribe()
    {
        _alertButtons.ForEach(x => x.RemoveListeners());
    }
}
