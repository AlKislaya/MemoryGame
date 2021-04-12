using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

//stores individual level object(card, paintables, static sprites)
public class LevelObjectController : MonoBehaviour
{
    private const float Pivot = 2.5f;
    public List<PaintableSpriteGroup> PaintableSpriteGroups => _paintableSpriteGroups;

    [SerializeField] private PaintableSpriteGroup _paintableSpriteGroupPrefab;
    [SerializeField] private SpriteRenderer _staticSpriteRendererPrefab;
    [HideInInspector] [SerializeField] private List<PaintableSpriteGroup> _paintableSpriteGroups = new List<PaintableSpriteGroup>();
    private float _localRotationZ;

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
        transform.localPosition = new Vector2(settings.Position.x - Pivot, Pivot - settings.Position.y);
        transform.localScale = new Vector3(settings.Scale.x, settings.Scale.y, 1);
        _localRotationZ = settings.Rotation;
        if (_localRotationZ != 0)
        {
            var rotation = transform.localRotation.eulerAngles;
            rotation.z = _localRotationZ;
            transform.localRotation = Quaternion.Euler(rotation);
        }

        //if static level object
        if (settings.PaintableColors == null || settings.PaintableColors.Count == 0)
        {
            OpenObject();
            return;
        }

        foreach (var paintableColor in settings.PaintableColors)
        {
            var paintable = _paintableSpriteGroups.FirstOrDefault(x => x.Key.Equals(paintableColor.Key));
            if (paintable == null)
            {
                Debug.LogError("Key " + paintableColor.Key + " is not represented.");
            }
            else
            {
                paintable.OriginalFillColor = paintableColor.Color;
                paintable.SetFillColor(paintableColor.Color);
            }
        }
    }

    public void OpenObject(bool isOpen = true)
    {
        transform.localRotation = Quaternion.Euler(new Vector3(0, isOpen ? 0 : 180, _localRotationZ));
    }

    public Tween OpenObjectAnimation(float duration, bool isOpen = true)
    {
        return DOTween.Sequence().Append(transform.DOLocalRotate(new Vector3(0, isOpen ? 0 : 180, _localRotationZ), duration));
    }

    public Tween PlaceObjectAnimation(float duration)
    {
        transform.localScale = new Vector3(1.1f, 1.1f, 1);
        gameObject.SetActive(false);

        return DOTween.Sequence().AppendCallback(() => gameObject.SetActive(true)).Append(transform.DOScale(new Vector3(1, 1, 1), duration));
    }
}