using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayableVectorSpritesController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public bool BlockingSpriteEnabled
    {
        get => _blockingSprite.enabled;
        set => _blockingSprite.enabled = value;
    }

    public bool ZoomEnabled
    {
        get => _zoom.enabled;
        set => _zoom.enabled = value;
    }

    //<new count, all>
    public event Action<int, int> OnFirstPaintedCountChanged;
    public event Action OnPaintableSpriteClicked;

    private const float RayDistance = 5f;
    private const float ClickDelay = .3f;

    [SerializeField] private PaintableSpriteGroup _paintableSpriteGroupPrefab;
    [SerializeField] private SpriteRenderer _staticSpriteRendererPrefab;
    [SerializeField] private SpriteRenderer _blockingSprite;
    [SerializeField] private Transform _spritesContainerPrefab;
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private Zoom _zoom;

    private Camera _cameraMain;
    private Transform _spritesContainer;
    private List<PaintableSpriteGroup> _paintableSpriteGroups = new List<PaintableSpriteGroup>();
    private SvgLoader _svgLoader;
    private ContactFilter2D _contactFilter;
    private VectorSpriteSettings _vectorSpriteSettings;

    //
    private PaintableSpriteGroup _lastClickedGroup;
    private float _pointerDownTime;
    private int _firstPaintedGroupsCount;
    //

    //assign non-changeable variables, fit in screen size
    private void Awake()
    {
        _cameraMain = Camera.main;
        _contactFilter = new ContactFilter2D() { layerMask = _raycastMask, useLayerMask = true };
        _vectorSpriteSettings = Settings.Instance.VectorSpriteSettings;
        FitInScreenSize();
    }

    public void LoadVectorSprite(TextAsset _svgAsset)
    {
        _svgLoader = new SvgLoader(_svgAsset);
        var z = 0f;
        //int order = 1;

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
                //staticSpriteInstance.sortingOrder = order;
                staticSpriteInstance.transform.localPosition = new Vector3(0, 0, z);
                //order++;
            }
            else if (vectorSprite is SvgLoader.PaintableVectorSpriteGroup paintableVectorGroup)
            {
                var paintableVectorInstance = Instantiate(_paintableSpriteGroupPrefab, _spritesContainer);
                paintableVectorInstance.transform.localPosition = new Vector3(0, 0, z);
                paintableVectorInstance.InitGroup(paintableVectorGroup/*, ref order*/);
                _paintableSpriteGroups.Add(paintableVectorInstance);
            }
        }
    }

    //reset variables, instantiate sprites container, assign in zoom, set base view settings
    public void Reset()
    {
        _lastClickedGroup = null;
        _pointerDownTime = 0;
        _firstPaintedGroupsCount = 0;

        _spritesContainer = Instantiate(_spritesContainerPrefab, transform);
        _zoom.AssignZoomContainer(_spritesContainer);

        ZoomEnabled = false;
        BlockingSpriteEnabled = true;
    }

    //destroy sprites container
    public void DestroyVectorSprites()
    {
        Destroy(_spritesContainer.gameObject);
        _paintableSpriteGroups = new List<PaintableSpriteGroup>();
    }

    //set color to the last clicked, check first painted
    public void SetLastClickedGroupColor(Color color)
    {
        if (_lastClickedGroup == null)
        {
            return;
        }

        _lastClickedGroup.SetFillColor(color);

        _lastClickedGroup.IsFirstPainted = true;
        if (_paintableSpriteGroups.Count(x => x.IsFirstPainted) != _firstPaintedGroupsCount)
        {
            _firstPaintedGroupsCount++;
            OnFirstPaintedCountChanged?.Invoke(_firstPaintedGroupsCount, _paintableSpriteGroups.Count);
        }
    }

    //remove and return colors
    public List<Color> ClearColors()
    {
        _paintableSpriteGroups.ForEach(x=>
        {
            x.SetActiveOriginalStroke(false);
            x.SetFillColor(_vectorSpriteSettings.ClearFillColor);
            x.SetStrokeColor(_vectorSpriteSettings.HighlightedStrokeColor);
        });
        return _paintableSpriteGroups.Select(x => x.OriginalFillColor).ToList();
    }

    //check paintables colors, set stroke colors, returns rightCount/totalCount
    public float CheckSprite()
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

        return (float)rightCount / _paintableSpriteGroups.Count;
    }

    //check click delay, generate a ray to fin objects under, invoke OnPaintableSpriteClicked if true
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - _pointerDownTime > ClickDelay)
        {
            return;
        }
        var hittedObjects = new List<RaycastHit2D>();

        if (Physics2D.Raycast(eventData.pointerCurrentRaycast.worldPosition, Vector2.zero, _contactFilter, hittedObjects, RayDistance) > 0)
        {
            var collider = hittedObjects[0].collider;
            foreach (var paintableSpriteGroup in _paintableSpriteGroups)
            {
                if (paintableSpriteGroup.ContainsCollider(collider))
                {
                    _lastClickedGroup = paintableSpriteGroup;
                    OnPaintableSpriteClicked?.Invoke();
                    return;
                }
            }
            Debug.Log("Did not Found a collider");
        }
    }

    //store press time
    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownTime = Time.time;
    }

    //scale transform to fit in screen
    private void FitInScreenSize()
    {
        var width = _cameraMain.orthographicSize * _cameraMain.aspect * 2;
        var vectorSpriteWidth = _vectorSpriteSettings.SceneRect.width / _vectorSpriteSettings.PixelsPerUnit;
        var scaleFactor = width / vectorSpriteWidth;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }
}