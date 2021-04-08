using System;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;

public class DefaultWindowAnimation : AWindowAnimation
{
    [SerializeField] private Canvas _canvas;
    private RectTransform _canvasRect;
    private static float _canvasWidth;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _canvasRect = _canvas.GetComponent<RectTransform>();

        if (_canvasWidth == 0)
        {
            _canvasWidth = ApplicationController.Instance.UiRoot.CanvasSize.x;
        }
    }

    public override void ShowImmediate()
    {
        _canvas.enabled = true;
    }

    public override void PlayShowAnimation(Action animationFinished = null)
    {
        _canvasRect.anchoredPosition = new Vector2(_canvasWidth, 0);
        DOTween.Sequence().Append(_canvasRect.DOAnchorPos(Vector2.zero, .5f))
            .AppendCallback(() => animationFinished?.Invoke());
    }

    public override void CloseImmediate()
    {
        _canvas.enabled = false;
    }

    public override void PlayCloseAnimation(Action animationFinished = null)
    {
        DOTween.Sequence().Append(_canvasRect.DOAnchorPos(new Vector2(_canvasWidth, 0), .5f))
            .AppendCallback(() => animationFinished?.Invoke());
    }
}