using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintableSpriteGroup : MonoBehaviour
{
    public Color OriginalFillColor => _originalFillColor;
    public Color CurrentColor => _paintableSprites[0].Color;
    public bool IsFirstPainted = false;
    [SerializeField] private PaintableSprite _paintableSpritePrefab;

    private List<PaintableSprite> _paintableSprites = new List<PaintableSprite>();
    private Color _originalFillColor;
    private int _originalStrokeOpacity;

    public void InitGroup(SvgLoader.PaintableVectorSpriteGroup paintableGroup/*, ref int order*/)
    {
        _originalFillColor = paintableGroup.OriginalFillColor;

        var z = -.001f;
        foreach (var paintableChild in paintableGroup.Children)
        {
            z -= .001f;
            var childInstance = Instantiate(_paintableSpritePrefab, transform);
            childInstance.transform.localPosition = childInstance.transform.localPosition + new Vector3(0, 0, z);
            childInstance.Init(paintableChild/*, ref order*/);
            _paintableSprites.Add(childInstance);
            childInstance.SetFillColor(_originalFillColor);
            //order++;
        }
    }

    public void SetFillColor(Color color)
    {
        _paintableSprites.ForEach(x => x.SetFillColor(color));
    }

    public void SetStrokeColor(Color color)
    {
        _paintableSprites.ForEach(x => x.SetStrokeColor(color));
    }

    public void SetActiveOriginalStroke(bool isActive)
    {
        _paintableSprites.ForEach(x => x.SetActiveOriginalStroke(isActive));
    }

    public bool ContainsCollider(Collider2D collider)
    {
        return _paintableSprites.Any(x => x.Collider2D == collider);
    }
}