using Dainty.UI.WindowBase;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : AWindowView
{
    [SerializeField] private Button _backgroundShadeButton;
    [SerializeField] private Button _closeButton;

    protected override void OnSubscribe()
    {
        base.OnSubscribe();
        _closeButton.onClick.AddListener(OnCloseClicked);
        _backgroundShadeButton.onClick.AddListener(OnCloseClicked);
    }
    protected override void OnUnSubscribe()
    {
        base.OnSubscribe();
        _backgroundShadeButton.onClick.RemoveListener(OnCloseClicked);
        _closeButton.onClick.RemoveListener(OnCloseClicked);
    }

    private void OnCloseClicked()
    {
        ApplicationController.Instance.UiManager.Back();
    }
}
