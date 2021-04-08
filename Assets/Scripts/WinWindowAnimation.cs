using System;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinWindowAnimation : AWindowAnimation
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _backgroundShade;
    [SerializeField] private RectTransform _panelContainer;
    private static float _canvasHalfHeight;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (_canvasHalfHeight == 0)
        {
            _canvasHalfHeight = ApplicationController.Instance.UiRoot.CanvasSize.y / 2;
        }
    }

    public override void ShowImmediate()
    {
        _canvas.enabled = true;
    }

    public override void PlayShowAnimation(Action animationFinished = null)
    {
        _panelContainer.anchoredPosition = new Vector2(0, _canvasHalfHeight);

        _backgroundShade.DOFade(.7f, .5f).SetEase(Ease.OutQuad);
        DOTween.Sequence().Append(_panelContainer.DOAnchorPos(Vector2.zero, .5f).SetEase(Ease.OutQuad))
            .AppendCallback(() => animationFinished?.Invoke());
    }

    public override void CloseImmediate()
    {
        _canvas.enabled = false;
    }

    public override void PlayCloseAnimation(Action animationFinished = null)
    {
        var color = _backgroundShade.color;
        color.a = 0;
        _backgroundShade.color = color;
        animationFinished?.Invoke();
    }
}