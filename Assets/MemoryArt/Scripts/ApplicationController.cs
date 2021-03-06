using Dainty.UI;
using MemoryArt.Global;
using MemoryArt.Global.Patterns;
using MemoryArt.UI.Windows;
using UnityEngine;

namespace MemoryArt
{
    public class ApplicationController : Singleton<ApplicationController>
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private UiRoot _uiRoot;
        [SerializeField] private UiManagerSettings _uiManagerSettings;
        [SerializeField] private TopPanelController _topPanelController;

#if UNITY_EDITOR || DEV
        [Header("[DEBUG]")] [SerializeField] private Tayx.Graphy.GraphyManager _graphyManagerPrefab;
        [SerializeField] private LunarConsolePlugin.LunarConsole _lunarConsolePrefab;
#endif

        private UiManager _uiManager;

        public Camera Camera => _camera;
        public UiManager UiManager => _uiManager;
        public UiRoot UiRoot => _uiRoot;
        public TopPanelController TopPanelController => _topPanelController;

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;

            _uiManager = new UiManager(_uiRoot, _uiManagerSettings);
            _uiManager.Open<MainWindowController>(false, WindowTransition.None);

#if DEV
            Instantiate(_lunarConsolePrefab);
            Instantiate(_graphyManagerPrefab);
#endif
        }
    }
}