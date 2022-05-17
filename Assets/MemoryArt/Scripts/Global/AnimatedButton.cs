using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button Button => _button;
    [SerializeField] private RectTransform _shadow;
    [SerializeField] private Transform _icon;
    [SerializeField] private Vector3 _targetScale;
    [SerializeField] private Vector3 _targetShadowPosition;
    [SerializeField] private float _duration;
    [SerializeField] private bool _isEnabled = true;

    private Button _button;
    private Vector3 _startScale;
    private Vector2 _startShadowPosition;
    private float _scaleDistance;
    private Sequence _animation;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Start()
    {
        _startScale = _icon.localScale;
        _startShadowPosition = _shadow.anchoredPosition;
        _scaleDistance = Mathf.Abs(_targetScale.x - _startScale.x);
        SetAnimationEnabled(_isEnabled);
    }

    public void SetAnimationEnabled(bool isEnabled)
    {
        _shadow.gameObject.SetActive(isEnabled);
        _isEnabled = isEnabled;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isEnabled)
        {
            return;
        }
        StartAnimation(_targetScale, _targetShadowPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isEnabled)
        {
            return;
        }
        StartAnimation(_startScale, _startShadowPosition);
    }

    private void StartAnimation(Vector3 targetScale, Vector3 targetPosition)
    {
        _animation?.Kill();

        float currDuration = Mathf.Abs((_icon.localScale.x - targetScale.x) * _duration / _scaleDistance);

        _animation = DOTween.Sequence()
            .Append(_icon.DOScale(targetScale, currDuration))
            .Join(_shadow.DOScale(targetScale, currDuration))
            .Join(_shadow.DOAnchorPos(targetPosition, currDuration)).Play();
    }
}