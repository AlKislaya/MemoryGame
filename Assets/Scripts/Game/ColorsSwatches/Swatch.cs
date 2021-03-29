using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Swatch : MonoBehaviour
{
    private const float AnimationDuration = .2f;

    [HideInInspector] public Color Color;
    public bool IsOn = false;

    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _swatch;
    [SerializeField] private RectTransform _shadow;
    [SerializeField] private Vector2 _targetShadowSize;
    private Sequence _shadowAnimation;
    private float _swatchStartYPos;

    private void Start()
    {
        _swatchStartYPos = _swatch.anchoredPosition.y;
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void Init(ToggleGroup toggleGroup, Sprite sprite, Color color)
    {
        _toggle.group = toggleGroup;
        _image.sprite = sprite;
        UpdateColor(color);
    }

    public void UpdateColor(Color color)
    {
        _image.color = color;
        Color = color;
    }

    private void OnToggleValueChanged(bool state)
    {
        IsOn = state;
        _shadowAnimation?.Kill();
        _shadowAnimation = DOTween.Sequence()
            .Append(_shadow.DOSizeDelta(state ? _targetShadowSize : Vector2.zero, AnimationDuration))
            .Join(_swatch.DOAnchorPosY(state ? _swatchStartYPos + _targetShadowSize.y : _swatchStartYPos, AnimationDuration)).Play();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
