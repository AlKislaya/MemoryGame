using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//stores individual level object(card, paintables, static sprites)
[Serializable]
public class LevelObjectController : MonoBehaviour
{
    public List<PaintableSpriteGroup> PaintableSpriteGroups => _paintableSpriteGroups;

    [SerializeField] private PaintableSpriteGroup _paintableSpriteGroupPrefab;
    [SerializeField] private SpriteRenderer _staticSpriteRendererPrefab;
    [HideInInspector] [SerializeField] private List<PaintableSpriteGroup> _paintableSpriteGroups = new List<PaintableSpriteGroup>();

    //instantiate and init sprites groups
    public void Init(List<SvgLoader.VectorSprite> vectorSprites)
    {
        var z = 0f;
        //int order = 1;

        foreach (var vectorSprite in vectorSprites)
        {
            z -= .01f;
            if (vectorSprite is SvgLoader.StaticVectorSprite staticVectorSprite)
            {
                var staticSpriteInstance = Instantiate(_staticSpriteRendererPrefab, transform);
                staticSpriteInstance.sprite = staticVectorSprite.Sprite;
                //staticSpriteInstance.sortingOrder = order;
                staticSpriteInstance.transform.localPosition = new Vector3(0, 0, z);
                //order++;
            }
            else if (vectorSprite is SvgLoader.PaintableVectorSpriteGroup paintableVectorGroup)
            {
                var paintableVectorInstance = Instantiate(_paintableSpriteGroupPrefab, transform);
                paintableVectorInstance.InitGroup(paintableVectorGroup/*, ref order*/);
                _paintableSpriteGroups.Add(paintableVectorInstance);
                paintableVectorInstance.transform.localPosition = new Vector3(0, 0, z);
            }
        }
    }

    //set object transform, original paintable groups colors, set original color
    public void InitSettings(LevelObjectSettings settings)
    {
        transform.localPosition = settings.Position;
        transform.localScale = new Vector3(settings.Scale.x, settings.Scale.y, 1);
        if (settings.Rotation != 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, settings.Rotation);
        }

        foreach (var paintableColor in settings.PaintableColors)
        {
            var paintable = _paintableSpriteGroups.FirstOrDefault(x => x.Key.Equals(paintableColor.Key));
            if (paintable == null)
            {
                Debug.LogError("Key "+ paintableColor.Key+" is not represented.");
            }
            else
            {
                paintable.OriginalFillColor = paintableColor.Color;
                paintable.SetFillColor(paintableColor.Color);
            }
        }
    }

    public void HideImage(bool isHiding = true)
    {
    }
}