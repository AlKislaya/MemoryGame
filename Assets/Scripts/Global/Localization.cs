using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LocalizationModule
{
    public class Localization : Singleton<Localization>
    {
        #region Local structure
        [Serializable]
        private class LanguagesWrapper
        {
            public List<Language> languages;
        }
        [Serializable]
        private class Language
        {
            public string languageKey;
            public List<LocalizationValue> localizationValues;
        }
        [Serializable]
        private class LocalizationValue
        {
            public string key;
            public string value;
        }
        #endregion

        private const string LocalPrefsKey = "local_key";

        public SystemLanguage CurrentLanguage => 
            _languageKey.Equals("EN") ? 
            SystemLanguage.English : SystemLanguage.Russian;

        public event Action OnLanguageChanged;

        [SerializeField] private TextAsset _json;

        private List<Language> _languages;
        private Language _currentLanguage;
        private string _languageKey;

        protected override void Awake()
        {
            base.Awake();

            _languages = JsonUtility.FromJson<LanguagesWrapper>(_json.text).languages;

            if (PlayerPrefs.HasKey(LocalPrefsKey))
            {
                _languageKey = PlayerPrefs.GetString(LocalPrefsKey);
            }
            else
            {
                _languageKey = Application.systemLanguage == SystemLanguage.Russian ? "RU" : "EN";
                SaveLocalKey();
            }

            _currentLanguage = _languages.First(x => x.languageKey.Equals(_languageKey));
        }

        public void ChangeLanguage(SystemLanguage language)
        {
            if (CurrentLanguage == language)
            {
                return;
            }

            _languageKey = language == SystemLanguage.Russian ? "RU" : "EN";

            SaveLocalKey();

            _currentLanguage = _languages.First(x => x.languageKey.Equals(_languageKey));

            OnLanguageChanged?.Invoke();
        }

        public string GetLocalByKey(string key)
        {
            var value = _currentLanguage.localizationValues.FirstOrDefault(x => x.key.Equals(key));

            if (value == null)
            {
                Debug.Log("Did not find key " + key);
                return "";
            }

            return value.value;
        }

        private void SaveLocalKey()
        {
            PlayerPrefs.SetString(LocalPrefsKey, _languageKey);
        }
    }
}
