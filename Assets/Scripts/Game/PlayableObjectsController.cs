using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PassedLevelStats
{
    public int PaintablesCount;
    public int RightPaintablesCount;
    public float Percents;
}

public class PlayableObjectsController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public int PaintablesCount => _paintableGroups.Count;

    public bool BlockingSpriteEnabled
    {
        set
        {
            if (_levelObjects != null && _levelObjects.Count > 0)
            {
                _levelObjects.ForEach(x => x.HideImage(value));
            }
        }
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

    [SerializeField] private LevelObjectController _levelObjectPrefab;
    [SerializeField] private Transform _spritesContainerPrefab;
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private Zoom _zoom;

    private Camera _cameraMain;
    private Transform _spritesContainer;
    private ContactFilter2D _contactFilter;
    private VectorSpriteSettings _vectorSpriteSettings;
    private List<LevelObjectController> _levelObjects = new List<LevelObjectController>();
    private List<PaintableSpriteGroup> _paintableGroups = new List<PaintableSpriteGroup>();

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

    public async Task LoadLevelObject(LevelObject levelObject)
    {
        //await Task.Delay((int)(Time.deltaTime * 100));
        var svgLoader = new SvgLoader(levelObject.SvgTextAsset);

        Debug.Log("LOADING STARTED "+ levelObject.SvgTextAsset.name);
        //tesselate and build sprites
        var vectorSprites = await svgLoader.GetSpritesArrange(_vectorSpriteSettings);
        Debug.Log("LOADING ENDED " + levelObject.SvgTextAsset.name);

        if (vectorSprites.Count == 0)
        {
            Debug.LogError("0 sprites");
            return;
        }


        for (int i = 0; i < levelObject.CopiesSettings.Count; i++)
        {
            var objectInstance = Instantiate(_levelObjectPrefab, _spritesContainer);

            objectInstance.Init(vectorSprites);
            objectInstance.InitSettings(levelObject.CopiesSettings[i]);
            _paintableGroups.AddRange(objectInstance.PaintableSpriteGroups);
            _levelObjects.Add(objectInstance);
        }

        //var objectInstance = Instantiate(_levelObjectPrefab, _spritesContainer);
        //objectInstance.Init(vectorSprites);
        //objectInstance.InitSettings(levelObject.CopiesSettings[0]);
        //_paintableGroups.AddRange(objectInstance.PaintableSpriteGroups);
        //_levelObjects.Add(objectInstance);

        //for (int i = 1; i < levelObject.CopiesSettings.Count; i++)
        //{
        //    var copySettings = levelObject.CopiesSettings[i];
        //    var objectCopyInstance = Instantiate(objectInstance, _spritesContainer);
        //    objectCopyInstance.InitSettings(copySettings);
        //    _paintableGroups.AddRange(objectCopyInstance.PaintableSpriteGroups);
        //    _levelObjects.Add(objectCopyInstance);
        //}
    }

    //reset variables, instantiate sprites container if null, assign in zoom, set base view settings
    public void SetDefaults()
    {
        _lastClickedGroup = null;
        _pointerDownTime = 0;
        _firstPaintedGroupsCount = 0;

        if (_spritesContainer == null)
        {
            _spritesContainer = Instantiate(_spritesContainerPrefab, transform);
        }
        else if (_paintableGroups != null && _paintableGroups.Count > 0)
        {
            _paintableGroups.ForEach(x => x.IsFirstPainted = false);
        }

        _zoom.AssignZoomContainer(_spritesContainer);

        ZoomEnabled = false;
        BlockingSpriteEnabled = true;
    }

    public void SetOriginalColors()
    {
        _paintableGroups.ForEach(x =>
        {
            x.SetActiveOriginalStroke(true);
            x.SetFillColor(x.OriginalFillColor);
        });
    }

    //destroy sprites container
    public void DestroyVectorSprites()
    {
        if (_spritesContainer == null)
        {
            return;
        }
        Destroy(_spritesContainer.gameObject);
        _spritesContainer = null;
        _levelObjects = new List<LevelObjectController>();
        _paintableGroups = new List<PaintableSpriteGroup>();
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
        if (_paintableGroups.Count(x=>x.IsFirstPainted) != _firstPaintedGroupsCount)
        {
            _firstPaintedGroupsCount++;
            OnFirstPaintedCountChanged?.Invoke(_firstPaintedGroupsCount, _paintableGroups.Count);
        }
    }

    //remove and return colors
    public List<Color> ClearColors()
    {
        var originalColors = new List<Color>();
       _paintableGroups.ForEach(x=>
        {
            x.SetActiveOriginalStroke(false);
            x.SetFillColor(_vectorSpriteSettings.ClearFillColor);
            x.SetStrokeColor(_vectorSpriteSettings.HighlightedStrokeColor);
            originalColors.Add(x.OriginalFillColor);
        });
        return originalColors;
    }

    //check paintables colors, set stroke colors, returns PassedLevelStats
    public PassedLevelStats CheckSprite()
    {
        int rightCount = 0;

        foreach (var paintableSpriteGroup in _paintableGroups)
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

        return new PassedLevelStats()
        {
            PaintablesCount = _paintableGroups.Count,
            RightPaintablesCount = rightCount,
            Percents = (float)rightCount / _paintableGroups.Count
        };
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
            foreach (var paintableSpriteGroup in _paintableGroups)
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