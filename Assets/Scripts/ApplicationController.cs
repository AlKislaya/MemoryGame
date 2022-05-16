using Dainty.Ads;
using Dainty.UI;
using UnityEngine;

public class ApplicationController : Singleton<ApplicationController>
{
    public Camera Camera => _camera;
    public UiManager UiManager => _uiManager;
    public UiRoot UiRoot => _uiRoot;
    public TopPanelController TopPanelController => _topPanelController;
    public IAdsController AdsController => _adsController;

    [SerializeField] private Camera _camera;
    [SerializeField] private UiRoot _uiRoot;
    [SerializeField] private UiManagerSettings _uiManagerSettings;
    [SerializeField] private TopPanelController _topPanelController;
    private UiManager _uiManager;
    private IAdsController _adsController;

    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;

        _uiManager = new UiManager(_uiRoot, _uiManagerSettings);
        _uiManager.Open<MainController>(false, WindowTransition.None);

        _adsController = CreateAdsController();
    }

    private IAdsController CreateAdsController()
    {
        var obj = new GameObject("MockAdsController");
        DontDestroyOnLoad(obj);

        IAdsController adsController = obj.AddComponent<EditorAdsController>();
        adsController.Initialize(true);
        return adsController;
    }
}