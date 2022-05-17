using TMPro;
using UnityEngine;

namespace LocalizationModule
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizableText : MonoBehaviour
    {
        public string Key
        {
            set
            {
                _key = value;
                UpdateLocal();
            }
        }

        [SerializeField] private string _key;

        private void Awake()
        {
            Localization.Instance.LanguageChanged += UpdateLocal;
            UpdateLocal();
        }

        private void UpdateLocal()
        {
            if (string.IsNullOrEmpty(_key))
            {
                Debug.LogWarning("No local key in " + gameObject.name);
                return;
            }

            GetComponent<TextMeshProUGUI>().text = Localization.Instance.GetLocalByKey(_key);
        }
    }
}