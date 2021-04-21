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
    private Tween _animation;
    private float _height;

    private void Awake()
    {
        _height = _rectTransform.sizeDelta.y;
        _backButton.onClick.AddListener(OnBackButtonClicked);
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
    }

    public void Close()
    {
        _animation?.Kill();
        _animation = _rectTransform.DOAnchorPosY(0, .2f).SetEase(Ease.OutSine);
    }
}
