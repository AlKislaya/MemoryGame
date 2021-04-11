using System;
using Dainty.UI;
using UnityEngine;

public class ApplicationController : Singleton<ApplicationController>
{
    public UiManager UiManager => _uiManager;
    public UiRoot UiRoot => _uiRoot;
    public TopPanelController TopPanelController => _topPanelController;

    [SerializeField] private UiRoot _uiRoot;
    [SerializeField] private UiManagerSettings _uiManagerSettings;
    [SerializeField] private TopPanelController _topPanelController;
    private UiManager _uiManager;

    protected override void Awake()
    {
        base.Awake();
        _uiManager = new UiManager(_uiRoot, _uiManagerSettings);
        _uiManager.Open<MainController>(false, WindowTransition.None);
    }

    public void SetActiveLoader(bool isActive)
    {
        //_loadingWindow.enabled = isActive;
        throw new NotImplementedException();
    }
}
