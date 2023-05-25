using System;
using System.Collections.Generic;
using System.Linq;
using MemoryArt.Global.Patterns;
using UnityEngine;
using YG;

namespace MemoryArt.Global
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

        [SerializeField] private TextAsset _json;

        private List<Language> _languages;
        private Language _currentLanguage;
        private string _languageKey;

        public SystemLanguage CurrentLanguage => GetSystemLanguageByCode(_languageKey);

        public event Action LanguageChanged;

        protected override void Awake()
        {
            base.Awake();
            
            _languages = JsonUtility.FromJson<LanguagesWrapper>(_json.text).languages;

#if UNITY_WEBGL
            _languageKey = YandexGame.savesData.LanguageKey;
            YandexGame.GetDataEvent += GetDataEvent;
#else
            if (PlayerPrefs.HasKey(LocalPrefsKey))
            {
                _languageKey = PlayerPrefs.GetString(LocalPrefsKey);
            }
            else
            {
                _languageKey = Application.systemLanguage == SystemLanguage.Russian ? "RU" : "EN";
                SaveLocalKey();
            }

#endif
            _currentLanguage = _languages.First(x => x.languageKey.Equals(_languageKey));
        }

        private void GetDataEvent()
        {
            ChangeLanguage(GetSystemLanguageByCode(YandexGame.savesData.LanguageKey));
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

            LanguageChanged?.Invoke();
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
#if UNITY_WEBGL
            YandexGame.savesData.LanguageKey = _languageKey;
            YandexGame.SaveProgress();
#else
            PlayerPrefs.SetString(LocalPrefsKey, _languageKey);
#endif
        }

        private SystemLanguage GetSystemLanguageByCode(string code)
        {
            return code.Equals("EN")
                ? SystemLanguage.English
                : SystemLanguage.Russian;
        }
        
#if UNITY_WEBGL
        private void OnDestroy()
        {
            YandexGame.GetDataEvent -= GetDataEvent;
        }
#endif
    }
}