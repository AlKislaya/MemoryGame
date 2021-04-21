using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StrokeColor
{
    Highlighted,
    Right,
    Wrong
}

public class PaintableSpriteGroup : MonoBehaviour
{
    private const float SortingStepZ = -.0001f;
    public string Key;
    public Color OriginalFillColor;
    public Color CurrentColor => _paintableSprites[0].Color;
    public bool IsFirstPainted = false;
    public bool IsRight => CurrentColor == OriginalFillColor;

    [SerializeField] private PaintableSprite _paintableSpritePrefab;
    [SerializeField] private Color _highlightedColor;
    [SerializeField] private Color _rightColor;
    [SerializeField] private Color _wrongColor;

    private List<PaintableSprite> _paintableSprites = new List<PaintableSprite>();

    public void InitGroup(SvgLoader.PaintableVectorSpriteGroup paintableGroup)
    {
        Key = paintableGroup.Key;
        var z = SortingStepZ;
        foreach (var paintableChild in paintableGroup.Children)
        {
            var childInstance = Instantiate(_paintableSpritePrefab, transform);
            childInstance.transform.localPosition = childInstance.transform.localPosition + new Vector3(0, 0, z);
            childInstance.Init(paintableChild);
            _paintableSprites.Add(childInstance);
            z += SortingStepZ;
        }
    }

    public void SetFillColor(Color color)
    {
        _paintableSprites.ForEach(x => x.SetFillColor(color));
    }

    public void HighlightStroke()
    {
        _paintableSprites.ForEach(x => x.SetStrokeColor(_highlightedColor));
    }

    //bug: if stroke will have alpha != 255 
    public void SetActiveOriginalStroke(bool isActive)
    {
        _paintableSprites.ForEach(x =>
        {
            x.SetActiveOriginalStroke(isActive);
            x.SetStrokeColor(Color.white);
        });
    }

    public Sequence DoStrokeCheckAnimation(float duration)
    {
        var color = IsRight ? _rightColor : _wrongColor;
        var targetA = color.a;
        color.a = 0;

        var sequence = DOTween.Sequence();
        foreach (var paintable in _paintableSprites)
        {
            paintable.SetStrokeColor(color);
            sequence.Join(paintable.DoStrokeAlpha(targetA, duration));
        }

        return sequence;
    }

    public bool ContainsCollider(Collider2D collider)
    {
        return _paintableSprites.Any(x => x.Collider2D == collider);
    }
}