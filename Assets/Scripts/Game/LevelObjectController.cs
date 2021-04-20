using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

//stores individual level object
public class LevelObjectController : MonoBehaviour
{
    private const float Pivot = 2.5f;
    private const float ScaleStep = .1f;
    private const float SortingStepZ = -.001f;
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
    public void InitSettings(LevelObjectSettings settings, LevelObjectSettings questionSignSettings, float zPos)
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

        InitQuestionSign(questionSignSettings);
        OpenObject(false);
    }

    public void SetGroupColorByKey(string key, Color color)
    {
        var groupByKey = _paintableSpriteGroups.FirstOrDefault(x => x.Key == key);
        if (groupByKey == null)
        {
            Debug.LogError("Key " + key + " is not represented in LO.");
            return;
        }

        groupByKey.OriginalFillColor = color;
        groupByKey.SetFillColor(color);
    }

    public void SetGroupsColor(Color color)
    {
        _paintableSpriteGroups.ForEach(x =>
        {
            x.OriginalFillColor = color;
            x.SetFillColor(color);
        });
    }

    private void InitQuestionSign(LevelObjectSettings questionSignSettings)
    {
        var questionInstance = Instantiate(_questionSignPrefab, transform);
        questionInstance.SetAsFirstSibling();

        questionInstance.localPosition = questionSignSettings.Position;

        if (questionSignSettings.Scale != Vector2.one)
        {
            questionInstance.localScale = new Vector3(questionSignSettings.Scale.x, questionSignSettings.Scale.y, 1);
        }

        if (questionSignSettings.Rotation != 0)
        {
            var rotation = questionInstance.localRotation.eulerAngles;
            rotation.z = questionSignSettings.Rotation;
            questionInstance.localRotation = Quaternion.Euler(rotation);
        }
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

    public Tween PlaceObjectAnimation(float duration)
    {
        transform.localScale = _scale + new Vector3(ScaleStep, ScaleStep, 0);
        gameObject.SetActive(false);

        return DOTween.Sequence().AppendCallback(() => gameObject.SetActive(true)).Append(transform.DOScale(_scale, duration));
    }
}