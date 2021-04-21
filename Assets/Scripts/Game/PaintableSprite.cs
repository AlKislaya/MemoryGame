using DG.Tweening;
using System;
using UnityEngine;

[Serializable]
public class PaintableSprite : MonoBehaviour
{
    public Collider2D Collider2D => _collider;
    public Color Color => _fillSpriteRenderer.color;

    [SerializeField] private SpriteRenderer _fillSpriteRenderer;
    [SerializeField] private SpriteRenderer _strokeSpriteRenderer;
    [HideInInspector] [SerializeField] private SvgLoader.PaintableVectorSprite _paintableVectorSprite;
    [HideInInspector] [SerializeField] private Collider2D _collider;

    public void Init(SvgLoader.PaintableVectorSprite paintableVectorSprite)
    {
        _fillSpriteRenderer.sprite = paintableVectorSprite.Fill;
        if (paintableVectorSprite.OriginalStroke != null)
        {
            _strokeSpriteRenderer.sprite = paintableVectorSprite.OriginalStroke;
        }
        var boxCollider = _fillSpriteRenderer.gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = paintableVectorSprite.Size;
        boxCollider.offset = paintableVectorSprite.Position;
        _collider = boxCollider;
        _paintableVectorSprite = paintableVectorSprite;
    }

    public void SetFillColor(Color color)
    {
        if (color == _fillSpriteRenderer.color)
        {
            return;
        }

        _fillSpriteRenderer.color = color;
    }

    public void SetStrokeColor(Color color)
    {
        if (_strokeSpriteRenderer.sprite == null || color == _strokeSpriteRenderer.color)
        {
            return;
        }

        _strokeSpriteRenderer.color = color;
    }

    public void SetActiveOriginalStroke(bool isActive)
    {
        _strokeSpriteRenderer.sprite = isActive ? _paintableVectorSprite.OriginalStroke : _paintableVectorSprite.Stroke;
    }

    public Tween DoStrokeAlpha(float targetAplpha, float duration)
    {
        return _strokeSpriteRenderer.DOFade(targetAplpha, duration);
    }

    //public static bool ContainsPoint(Vector3[] polyPoints, Vector2 p)
    //{
    //    var j = polyPoints.Length - 1;
    //    var inside = false;
    //    for (int i = 0; i < polyPoints.Length; j = i++)
    //    {
    //        var pi = polyPoints[i];
    //        var pj = polyPoints[j];
    //        if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
    //            (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
    //            inside = !inside;
    //    }
    //    return inside;
    //}
}