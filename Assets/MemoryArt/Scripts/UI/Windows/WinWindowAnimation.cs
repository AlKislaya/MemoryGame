using System;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinWindowAnimation : AWindowAnimation
{
    private const float _animationDuration = .4f;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _backgroundShade;
    [SerializeField] private RectTransform _panelContainer;

    private Tween _tween;
    private static float _canvasHalfHeight;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (_canvasHalfHeight == 0)
        {
            _canvasHalfHeight = UIRoot.CanvasSize.y / 2;
        }
    }

    public override void ShowImmediate()
    {
        _tween?.Kill();
        _canvas.enabled = true;
    }

    public override void PlayShowAnimation(bool push, Action animationFinished = null)
    {
        _tween?.Kill();
        _panelContainer.anchoredPosition = new Vector2(0, _canvasHalfHeight);

        _backgroundShade.DOFade(.7f, _animationDuration).SetEase(Ease.OutQuad);
        _tween = DOTween.Sequence()
            .Append(_panelContainer.DOAnchorPos(Vector2.zero, _animationDuration).SetEase(Ease.OutQuad))
            .AppendCallback(() => animationFinished?.Invoke());
    }

    public override void CloseImmediate()
    {
        _tween?.Kill();
        _canvas.enabled = false;
    }

    public override void PlayCloseAnimation(bool pop, Action animationFinished = null)
    {
        _tween?.Kill();
        var color = _backgroundShade.color;
        color.a = 0;
        _backgroundShade.color = color;
        animationFinished?.Invoke();
    }
}