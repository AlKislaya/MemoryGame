using Dainty.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelController : MonoBehaviour
{
    public bool IsBackButtonInteractable
    {
        get => _backButton.interactable;
        set => _backButton.interactable = value;
    }

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private Button _backButton;
    [SerializeField] private float _shadowHeight;

    [Header("Safe Area")] [SerializeField] RectTransform _canvasRect;
    [SerializeField] SafeArea _safeArea;

    private Tween _animation;
    private bool _isClosed = true;
    private float _topOffset;
    private float _height;

    private void Awake()
    {
        _height = _rectTransform.sizeDelta.y;
        _backButton.onClick.AddListener(OnBackButtonClicked);

        _safeArea.Changed += OnSafeAreaChanged;
    }

    private void OnBackButtonClicked()
    {
        ApplicationController.Instance.UiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }

    public void Show(string header)
    {
        _header.text = header;
        _animation?.Kill();
        _animation = _rectTransform.DOAnchorPosY(-_height, .5f).SetEase(Ease.InSine);
        _isClosed = false;
    }

    public void Close()
    {
        _animation?.Kill();
        _animation = _rectTransform.DOAnchorPosY(_topOffset, .2f).SetEase(Ease.OutSine);
        _isClosed = true;
    }

    private void OnSafeAreaChanged()
    {
        var safeArea = ApplicationController.Instance.UiRoot.GetSafeArea();
        _topOffset = _canvasRect.rect.height - (safeArea.height + safeArea.y) + _shadowHeight;

        var size = _rectTransform.sizeDelta;
        size.y = _height + _topOffset;
        _rectTransform.sizeDelta = size;

        if (_isClosed)
        {
            _animation?.Kill();
            var pos = _rectTransform.anchoredPosition;
            pos.y = _topOffset;
            _rectTransform.anchoredPosition = pos;
        }
    }
}
