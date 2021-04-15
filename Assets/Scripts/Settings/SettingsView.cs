using Dainty.UI.WindowBase;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : AWindowView
{
    [SerializeField] private Button _backgroundShadeButton;

    protected override void OnSubscribe()
    {
        base.OnSubscribe();
        _backgroundShadeButton.onClick.AddListener(OnBackgroundShadeClicked);
    }
    protected override void OnUnSubscribe()
    {
        base.OnSubscribe();
        _backgroundShadeButton.onClick.RemoveListener(OnBackgroundShadeClicked);
    }

    private void OnBackgroundShadeClicked()
    {
        ApplicationController.Instance.UiManager.Back();
    }
}
