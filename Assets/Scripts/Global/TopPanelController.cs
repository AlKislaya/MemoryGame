using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopPanelController : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _header;
    private Tween _animation;
    private float _height;

    private void Awake()
    {
        _height = _rectTransform.sizeDelta.y;
    }

    public void Show(string header)
    {
        _header.text = header;
        _animation?.Kill();
        _animation = _rectTransform.DOAnchorPosY(0, .5f).SetEase(Ease.InSine);
    }

    public void Close()
    {
        _animation?.Kill();
        _animation = _rectTransform.DOAnchorPosY(_height, .2f).SetEase(Ease.OutSine);
    }
}
