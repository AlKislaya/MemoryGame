using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CounterElement : MonoBehaviour
{
    public float FillAmount => _filledOutline.fillAmount;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _filledOutline;
    [SerializeField] private GameObject _shadow;
    [SerializeField] private Button _button;

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetAmount(float fillAmount)
    {
        _filledOutline.fillAmount = fillAmount;
    }

    public void SetOutlineColor(Color color)
    {
        _filledOutline.color = color;
    }

    public void TransformToButton(string text, UnityAction OnButtonClicked)
    {
        _shadow.SetActive(false);
        _text.text = text;
        _button.gameObject.SetActive(true);

        _filledOutline.type = Image.Type.Sliced;
        var outlineRect = _filledOutline.GetComponent<RectTransform>();
        outlineRect.localScale = new Vector3(.6f, .6f, 1);
        outlineRect.sizeDelta = new Vector2(210, outlineRect.sizeDelta.y);
        _button.onClick.AddListener(OnButtonClicked);
    }
}