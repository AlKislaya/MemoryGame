using System;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;

public class DefaultWindowAnimation : AWindowAnimation
{
    [SerializeField] private Canvas _canvas;
    private RectTransform _canvasRect;
    private static float _canvasWidth;
    private Tween _tween;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _canvasRect = _canvas.GetComponent<RectTransform>();

        if (_canvasWidth == 0)
        {
            _canvasWidth = ApplicationController.Instance.UiRoot.CanvasSize.x;
            DOTween.Sequence()
                .AppendInterval(.5f)
                .AppendCallback(() => _canvasWidth = ApplicationController.Instance.UiRoot.CanvasSize.x)
                .Play();
        }
    }

    public override void ShowImmediate()
    {
        _tween?.Kill();
        _canvasRect.anchoredPosition = Vector2.zero;
        _canvas.enabled = true;
    }

    public override void PlayShowAnimation(bool push, Action animationFinished = null)
    {
        _tween?.Kill();
        _canvasRect.anchoredPosition = new Vector2(push ? _canvasWidth : -_canvasWidth, 0);
        _tween = DOTween.Sequence().Append(_canvasRect.DOAnchorPos(Vector2.zero, .5f))
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
        _tween = DOTween.Sequence().Append(_canvasRect.DOAnchorPos(new Vector2(pop ? _canvasWidth : -_canvasWidth, 0), .5f))
            .AppendCallback(() => animationFinished?.Invoke());
    }
}