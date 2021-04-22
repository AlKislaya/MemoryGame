using Dainty.UI.WindowBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : AWindowView
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _backgroundShadeButton;
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
