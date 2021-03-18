using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayableVectorSpritesController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    //<new count, all>
    public event Action<int, int> OnFirstClickedCountChanged;

    private const float RayDistance = 5f;

    [SerializeField] private PaintableSpriteGroup _paintableSpriteGroupPrefab;
    [SerializeField] private SpriteRenderer _staticSpriteRendererPrefab;
    [SerializeField] private Transform _spritesContainer;
    [SerializeField] private ColorsSwatchesController _colorsController;
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private Camera _cameraMain;

    private List<PaintableSpriteGroup> _paintableSpriteGroups = new List<PaintableSpriteGroup>();
    private SvgLoader _svgLoader;
    private ContactFilter2D _contactFilter;
    private VectorSpriteSettings _vectorSpriteSettings;

    //
    private float _pointerDownTime;
    private int _firstClickedCount = 0;
    //

    private void Awake()
    {
        _contactFilter = new ContactFilter2D() { layerMask = _raycastMask, useLayerMask = true };
        _vectorSpriteSettings = Settings.Instance.VectorSpriteSettings;
        FitInScreenSize();
    }

    public void LoadVectorSprite(TextAsset _svgAsset)
    {
        _svgLoader = new SvgLoader(_svgAsset);
        var z = 0f;
        int order = 1;

        var vectorSprites = _svgLoader.GetSpritesArrange(_vectorSpriteSettings);
        if (vectorSprites.Count == 0)
        {
            Debug.LogError("0 sprites");
            return;
        }

        foreach (var vectorSprite in vectorSprites)
        {
            z -= .01f;
            if (vectorSprite is SvgLoader.StaticVectorSprite staticVectorSprite)
            {
                var staticSpriteInstance = Instantiate(_staticSpriteRendererPrefab, _spritesContainer);
                staticSpriteInstance.sprite = staticVectorSprite.Sprite;
                staticSpriteInstance.sortingOrder = order;
                staticSpriteInstance.transform.localPosition = new Vector3(0, 0, z);
                order++;
            }
            else if (vectorSprite is SvgLoader.PaintableVectorSpriteGroup paintableVectorGroup)
            {
                var paintableVectorInstance = Instantiate(_paintableSpriteGroupPrefab, _spritesContainer);
                paintableVectorInstance.transform.localPosition = new Vector3(0, 0, z);
                paintableVectorInstance.InitGroup(paintableVectorGroup, ref order);
                _paintableSpriteGroups.Add(paintableVectorInstance);
            }
        }
    }

    public void ClearColors()
    {
        _paintableSpriteGroups.ForEach(x=>
        {
            x.SetActiveOriginalStroke(false);
            x.SetFillColor(_vectorSpriteSettings.ClearFillColor);
            x.SetStrokeColor(_vectorSpriteSettings.HighlightedStrokeColor);
        });

        _colorsController.AddColors(_paintableSpriteGroups.Select(x => x.OriginalFillColor).ToList());
    }

    public int CheckSprite()
    {
        int rightCount = 0;

        foreach (var paintableSpriteGroup in _paintableSpriteGroups)
        {
            if (paintableSpriteGroup.CurrentColor == paintableSpriteGroup.OriginalFillColor)
            {
                paintableSpriteGroup.SetStrokeColor(_vectorSpriteSettings.RightStrokeColor);
                rightCount++;
            }
            else
            {
                paintableSpriteGroup.SetStrokeColor(_vectorSpriteSettings.WrongStrokeColor);
            }
        }

        return rightCount;
    }

    public void ShowSwatches()
    {
        _colorsController.Show();
    }

    private void FitInScreenSize()
    {
        var width = _cameraMain.orthographicSize * _cameraMain.aspect * 2;
        var vectorSpriteWidth = _vectorSpriteSettings.SceneRect.width / _vectorSpriteSettings.PixelsPerUnit;
        var scaleFactor = width / vectorSpriteWidth;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - _pointerDownTime > .3f)
        {
            return;
        }
        var hittedObjects = new List<RaycastHit2D>();

        if (Physics2D.Raycast(eventData.pointerCurrentRaycast.worldPosition, Vector2.zero, _contactFilter, hittedObjects, RayDistance) > 0)
        {
            var color = Color.white;
            if (!_colorsController.TryGetSelectedColor(ref color))
            {
                return;
            }

            var collider = hittedObjects[0].collider;
            foreach (var paintableSpriteGroup in _paintableSpriteGroups)
            {
                if (paintableSpriteGroup.ContainsCollider(collider))
                {
                    paintableSpriteGroup.SetFillColor(color);
                    paintableSpriteGroup.IsFirstClicked = true;
                    if (_paintableSpriteGroups.Count(x => x.IsFirstClicked) != _firstClickedCount)
                    {
                        _firstClickedCount++;
                        OnFirstClickedCountChanged?.Invoke(_firstClickedCount, _paintableSpriteGroups.Count);
                    }
                    return;
                }
            }
            Debug.Log("Did not Found a collider");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownTime = Time.time;
    }
}