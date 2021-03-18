using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayableVectorSpritesController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerDownHandler
{
    //<new count, all>
    public event Action<int, int> OnFirstClickedCountChanged;

    private const float RayDistance = 5f;
    private const float TouchZoomSpeed = 0.005f;

    [SerializeField] private PaintableSpriteGroup _paintableSpriteGroupPrefab;
    [SerializeField] private SpriteRenderer _staticSpriteRendererPrefab;
    [SerializeField] private Transform _spritesContainer;
    [SerializeField] private ColorsSwatchesController _colorsController;
    [SerializeField] private LayerMask _raycastMask;

    private List<PaintableSpriteGroup> _paintableSpriteGroups = new List<PaintableSpriteGroup>();
    private SvgLoader _svgLoader;
    private ContactFilter2D _contactFilter;
    private VectorSpriteSettings _vectorSpriteSettings;

    //
    private float _zoomMinBound = 1f;
    private float _zoomMaxBound = 3f;

    private float _halfWidth;
    private float _halfHeight;

    private Vector3 _lastDragPos;

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

    public void OnDrag(PointerEventData eventData)
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
#if !UNITY_EDITOR
        if (Input.touchCount == 1)
        {
#endif
            var newPos = _spritesContainer.localPosition - (_lastDragPos - worldPosition);
            _lastDragPos = worldPosition;
            newPos.z = _spritesContainer.localPosition.z;
            ClampContainerInRect(newPos);
#if !UNITY_EDITOR
        }
        else
        if (Input.touchCount == 2)
        {
            // get current touch positions
            Touch tZero = Input.GetTouch(0);
            Touch tOne = Input.GetTouch(1);
            // get touch position from the previous frame
            Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
            Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

            float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
            float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

            // get offset value
            float deltaDistance = oldTouchDistance - currentTouchDistance;
            Zoom(deltaDistance, TouchZoomSpeed);
        }
#endif
    }

    void Zoom(float deltaMagnitudeDiff, float speed)
    {
        _spritesContainer.localScale = new Vector3(
            Mathf.Clamp(_spritesContainer.localScale.x - deltaMagnitudeDiff * speed, _zoomMinBound, _zoomMaxBound),
            Mathf.Clamp(_spritesContainer.localScale.y - deltaMagnitudeDiff * speed, _zoomMinBound, _zoomMaxBound), 
            0);

        ClampContainerInRect(_spritesContainer.localPosition);
    }

    private void ClampContainerInRect(Vector3 position)
    {
        var x = _halfWidth * _spritesContainer.localScale.x - _halfWidth;
        var y = _halfHeight * _spritesContainer.localScale.y - _halfHeight;
        _spritesContainer.localPosition = new Vector3(
            Mathf.Clamp(position.x, -x, x),
            Mathf.Clamp(position.y, -y, y),
            position.z);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastDragPos = eventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _lastDragPos = eventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    private void FitInScreenSize()
    {
        var width = Camera.main.orthographicSize * Camera.main.aspect * 2;
        var vectorSpriteWidth = _vectorSpriteSettings.SceneRect.width / _vectorSpriteSettings.PixelsPerUnit;
        var scaleFactor = width / vectorSpriteWidth;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

        _halfWidth = vectorSpriteWidth / 2;
        _halfHeight = _vectorSpriteSettings.SceneRect.height / _vectorSpriteSettings.PixelsPerUnit / 2;
        _zoomMinBound = Settings.Instance.GameSettings.MinZoomValue;
        _zoomMaxBound = Settings.Instance.GameSettings.MaxZoomValue;
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