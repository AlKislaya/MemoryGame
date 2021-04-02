using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PaintableSpriteGroup : MonoBehaviour
{
    [HideInInspector] [SerializeField] public string Key;
    [HideInInspector] [SerializeField] public Color OriginalFillColor;
    public Color CurrentColor => _paintableSprites[0].Color;
    public bool IsFirstPainted = false;
    [SerializeField] private PaintableSprite _paintableSpritePrefab;

    [HideInInspector] [SerializeField] private List<PaintableSprite> _paintableSprites = new List<PaintableSprite>();

    public void InitGroup(SvgLoader.PaintableVectorSpriteGroup paintableGroup/*, ref int order*/)
    {
        Key = paintableGroup.Key;
        var z = -.001f;
        foreach (var paintableChild in paintableGroup.Children)
        {
            z -= .001f;
            var childInstance = Instantiate(_paintableSpritePrefab, transform);
            childInstance.transform.localPosition = childInstance.transform.localPosition + new Vector3(0, 0, z);
            childInstance.Init(paintableChild/*, ref order*/);
            _paintableSprites.Add(childInstance);
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