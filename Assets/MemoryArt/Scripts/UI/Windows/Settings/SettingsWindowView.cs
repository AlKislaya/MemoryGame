using Dainty.UI.WindowBase;
using LocalizationModule;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindowView : AWindowView
{
    [Serializable]
    public class LanguageToggle
    {
        public SystemLanguage Language;
        public Toggle Toggle;
    }

    [SerializeField] private GameObject _mainContainer;
    [SerializeField] private Button _backgroundShadeButton;
    [SerializeField] private Button _closeButton;
    [Header("Language")]
    [SerializeField] private GameObject _languageContainer;
    [SerializeField] private TextMeshProUGUI _languageKey;
    [SerializeField] private Button _languageButton;
    [SerializeField] private Button _languageBackButton;
    [SerializeField] private ToggleGroup _languageToggleGroup;
    [SerializeField] private List<LanguageToggle> _languageToggles;

    public event Action CloseButtonClick;

    private void Start()
    {
        _languageBackButton.onClick.AddListener(OpenMainContainer);
        Localization.Instance.OnLanguageChanged += UpdateLocalKey;
        UpdateLocalKey();

        var currLanguage = Localization.Instance.CurrentLanguage;
        foreach (var toggle in _languageToggles)
        {
            if (toggle.Language == currLanguage)
            {
                toggle.Toggle.SetIsOnWithoutNotify(true);
                break;
            }
        }
    }

    public void SetDefaults()
    {
        OpenMainContainer();
    }

    private void UpdateLocalKey()
    {
        _languageKey.text = Localization.Instance.CurrentLanguage == SystemLanguage.English ? "En" : "Ру";
    }

    protected override void OnSubscribe()
    {
        base.OnSubscribe();
        _closeButton.onClick.AddListener(OnCloseClicked);
        _backgroundShadeButton.onClick.AddListener(OnCloseClicked);
        _languageButton.onClick.AddListener(OpenLanguageContainer);
        _languageToggles.ForEach(x => x.Toggle.onValueChanged.AddListener(OnToggleValueChanged));
    }

    protected override void OnUnSubscribe()
    {
        base.OnSubscribe();
        _backgroundShadeButton.onClick.RemoveListener(OnCloseClicked);
        _closeButton.onClick.RemoveListener(OnCloseClicked);
        _languageButton.onClick.RemoveListener(OpenLanguageContainer);
        _languageToggles.ForEach(x => x.Toggle.onValueChanged.RemoveListener(OnToggleValueChanged));
    }

    private void OnToggleValueChanged(bool isOn)
    {
        var selectedToggle = _languageToggleGroup.GetFirstActiveToggle();
        foreach (var toggle in _languageToggles)
        {
            if (toggle.Toggle == selectedToggle)
            {
                Localization.Instance.ChangeLanguage(toggle.Language);
                break;
            }
        }
    }

    private void OpenLanguageContainer()
    {
        _mainContainer.SetActive(false);
        _languageContainer.SetActive(true);
    }

    private void OpenMainContainer()
    {
        _mainContainer.SetActive(true);
        _languageContainer.SetActive(false);
    }

    private void OnCloseClicked()
    {
        CloseButtonClick?.Invoke();
    }
}
