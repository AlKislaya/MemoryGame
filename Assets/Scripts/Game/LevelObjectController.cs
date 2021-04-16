using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

//stores individual level object(card, paintables, static sprites)
public class LevelObjectController : MonoBehaviour
{
    private const float Pivot = 2.5f;
    private const float ScaleStep = .1f;
    private const float SortingStepZ = -.01f;
    public List<PaintableSpriteGroup> PaintableSpriteGroups => _paintableSpriteGroups;

    [SerializeField] private PaintableSpriteGroup _paintableSpriteGroupPrefab;
    [SerializeField] private SpriteRenderer _staticSpriteRendererPrefab;
    [SerializeField] private Transform _questionSignPrefab;
    private List<PaintableSpriteGroup> _paintableSpriteGroups = new List<PaintableSpriteGroup>();
    private float _localRotationZ;
    private Vector3 _scale;

    //instantiate and init sprites groups
    public void Init(List<SvgLoader.VectorSprite> vectorSprites)
    {
        //int order = 1;

        foreach (var vectorSprite in vectorSprites)
        {
            if (vectorSprite is SvgLoader.StaticVectorSprite staticVectorSprite)
            {
                var staticSpriteInstance = Instantiate(_staticSpriteRendererPrefab, transform);
                staticSpriteInstance.sprite = staticVectorSprite.Sprite;
                //staticSpriteInstance.sortingOrder = order;
                //order++;
            }
            else if (vectorSprite is SvgLoader.PaintableVectorSpriteGroup paintableVectorGroup)
            {
                var paintableVectorInstance = Instantiate(_paintableSpriteGroupPrefab, transform);
                paintableVectorInstance.InitGroup(paintableVectorGroup/*, ref order*/);
                _paintableSpriteGroups.Add(paintableVectorInstance);
            }
        }
    }

    //set object transform, original paintable groups colors, set original color
    public void InitSettings(LevelObjectSettings settings, float zPos)
    {
        transform.localPosition = new Vector3(settings.Position.x - Pivot, Pivot - settings.Position.y, zPos);
        _scale = new Vector3(settings.Scale.x, settings.Scale.y, 1);
        transform.localScale = _scale;
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

        var paintablesCount = _paintableSpriteGroups.Count;
        for (int i = 0; i < settings.PaintableColors.Count; i++)
        {
            PaintableSpriteGroup paintable;
            var colorSettings = settings.PaintableColors[i];

            if (!colorSettings.SearchByKey && i < paintablesCount)
            {
                paintable = _paintableSpriteGroups[i];
            }
            else
            {
                paintable = _paintableSpriteGroups.FirstOrDefault(x => x.Key.Equals(colorSettings.Key));
            }

            if (paintable == null)
            {
                Debug.LogError("Key " + colorSettings.Key + " is not represented in LO.");
            }
            else
            {
                paintable.OriginalFillColor = colorSettings.Color;
                paintable.SetFillColor(colorSettings.Color);
            }
        }

        Instantiate(_questionSignPrefab, transform).SetAsFirstSibling();
        OpenObject(false);
    }

    public void OpenObject(bool isOpen = true)
    {
        //transform.localRotation = Quaternion.Euler(new Vector3(0, isOpen ? 0 : 180, _localRotationZ));
        var childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            var childPosition = transform.GetChild(i).transform.localPosition;
            childPosition.z = isOpen ? i * SortingStepZ : (childCount - i) * SortingStepZ;
            transform.GetChild(i).transform.localPosition = childPosition;
        }
    }

    public Sequence OpenObjectAnimation(float duration, bool isOpen = true)
    {
        var sequence = DOTween.Sequence();
        var childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            sequence.Append(transform.GetChild(i).transform.DOLocalMoveZ(isOpen ? i * SortingStepZ : (childCount - i) * SortingStepZ, .01f));
        }
        return sequence;
        //return DOTween.Sequence().AppendCallback(()=> transform.rotation = Quaternion.Euler(new Vector3(0, isOpen ? 360 : -180, _localRotationZ)));
        //return DOTween.Sequence().Append(transform.DOLocalRotate(new Vector3(0, isOpen ? 360 : -180, _localRotationZ), duration));
    }

    public Tween PlaceObjectAnimation(float duration)
    {
        transform.localScale = _scale + new Vector3(ScaleStep, ScaleStep, 0);
        gameObject.SetActive(false);

        return DOTween.Sequence().AppendCallback(() => gameObject.SetActive(true)).Append(transform.DOScale(_scale, duration));
    }
}