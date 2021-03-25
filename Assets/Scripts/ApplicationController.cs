using Dainty.UI;
using UnityEngine;

public class ApplicationController : Singleton<ApplicationController>
{
    public UiManager UiManager => _uiManager;

    [SerializeField] private UiRoot _uiRoot;
    [SerializeField] private UiManagerSettings _uiManagerSettings;
    private UiManager _uiManager;

    protected override void Awake()
    {
        base.Awake();
        _uiManager = new UiManager(_uiRoot, _uiManagerSettings);
        _uiManager.Open<LevelsSequenceController>();
    }
}
