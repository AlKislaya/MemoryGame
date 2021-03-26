using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CounterElement : MonoBehaviour
{
    public event Action OnButtonClicked; 
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _filledOutline;
    [SerializeField] private GameObject _shadow;
    [SerializeField] private Button _button;
    [SerializeField] private Sprite _circleSprite;
    [SerializeField] private Sprite _buttonSprite;
    [SerializeField] private Vector2 _targetButtonSizeDelta = new Vector2(175, 50);

    private Vector2 _initialSizeDelta;
    private RectTransform _outlineRect;
    private Tween _animation;
    private Tween _transformToButtonAnimation;

    //init _outlineRect and sizes
    private void Awake()
    {
        _outlineRect = _filledOutline.GetComponent<RectTransform>();
        _initialSizeDelta = _outlineRect.sizeDelta;
    }

    //add button click listener, init transform to btn animation
    private void Start()
    {
        _button.onClick.AddListener(onButtonClicked);
        _transformToButtonAnimation = DOTween.Sequence()
            .AppendCallback(() =>
            {
                _shadow.SetActive(false);
                _filledOutline.type = Image.Type.Sliced;
                //_text.text = text;
            })
            .Append(_outlineRect.DOSizeDelta(new Vector2(_targetButtonSizeDelta.y, _targetButtonSizeDelta.y), .2f).SetEase(Ease.InSine))
            .AppendCallback(() => _filledOutline.sprite = _buttonSprite)
            .Append(_outlineRect.DOSizeDelta(_targetButtonSizeDelta, .3f).SetEase(Ease.InSine))
            .AppendCallback(() => _button.enabled = true).Pause().SetAutoKill(false);
    }

    private void onButtonClicked()
    {
        OnButtonClicked?.Invoke();
    }

    public void Reset()
    {
        _filledOutline.type = Image.Type.Filled;
        _filledOutline.sprite = _circleSprite;
        _outlineRect.sizeDelta = _initialSizeDelta;
        _shadow.SetActive(true);
        _button.enabled = false;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetAmount(float fillAmount, float duration = 0, Ease ease = Ease.Linear)
    {
        _animation?.Kill();
        if (duration == 0)
        {
            _filledOutline.fillAmount = fillAmount;
        }
        else
        {
            _animation = _filledOutline.DOFillAmount(fillAmount, duration).SetEase(ease);
            _animation.Play();
        }
    }

    public void SetOutlineColor(Color color)
    {
        _filledOutline.color = color;
    }

    public void TransformToButton(string text)
    {
        if (_animation != null && _animation.IsPlaying())
        {
            _animation.OnComplete(() => _transformToButtonAnimation.Restart());
        }
        else
        {
            _transformToButtonAnimation.Restart();
        }
    }
}