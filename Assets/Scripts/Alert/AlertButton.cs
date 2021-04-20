using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;

    public void Initialize(AlertButtonSettings buttonSettings)
    {
        _text.text = buttonSettings.Text;
        _image.color = buttonSettings.Color;
        _button.onClick.AddListener(buttonSettings.Callback);
    }
    
    public void RemoveListeners()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
