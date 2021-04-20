using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
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
    private const float SortingStepZ = -.1f;
    private const float RayDistance = 5f;
    private const float ClickDelay = .3f;

    public int PaintablesCount => _paintableGroups.Count;
    public bool SpriteMaskEnabled
    {
        set => _spriteMask.enabled = value;
    }

    public bool ZoomEnabled
    {
        get => _zoom.enabled;
        set => _zoom.enabled = value;
    }

    //<new count, all>
    public event Action<int, int> OnFirstPaintedCountChanged;
    public event Action OnPaintableSpriteClicked;

    [SerializeField] private LevelObjectsController _levelObjectsPrefab;
    [SerializeField] private Transform _levelObjectsContainerPrefab;
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private SpriteMask _spriteMask;
    [SerializeField] private Zoom _zoom;

    private Camera _cameraMain;
    private Transform _objectsContainer;
    private ContactFilter2D _contactFilter;
    private VectorSpriteSettings _levelObjectsSettings;
    private List<LevelObjectsController> _levelObjectsControllers = new List<LevelObjectsController>();
    private List<PaintableSpriteGroup> _paintableGroups = new List<PaintableSpriteGroup>();

    //
    private PaintableSpriteGroup _lastClickedGroup;
    private float _pointerDownTime;
    private int _firstPaintedGroupsCount;
    //

    //assign non-changeable variables, fit in screen size
    private void Awake()
    {
        _levelObjectsSettings = Settings.Instance.VectorSpriteSettings;
        _cameraMain = Camera.main;
        _contactFilter = new ContactFilter2D() { layerMask = _raycastMask, useLayerMask = true };
        FitInScreenSize();
    }

    public async Task LoadLevel(Level levelAsset, CancellationToken token)
    {
        var zPos = SortingStepZ;
        foreach (var levelObject in levelAsset.LevelObjects)
        {
            if (token.IsCancellationRequested)
            {
                Debug.Log("Canceled Loading level");
                return;
            }

            //check copies count in object
            if (levelObject.CopiesSettings == null || levelObject.CopiesSettings.Count == 0)
            {
                Debug.LogError("No copies in " + levelObject.SvgTextAsset.name);
                continue;
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            var levelObjsController = Instantiate(_levelObjectsPrefab, _objectsContainer);
            levelObjsController.transform.localPosition = new Vector3(0, 0, zPos);
            if (!levelObject.IsStatic)
            {
                _levelObjectsControllers.Add(levelObjsController);
            }

            var paintableGroups = await levelObjsController.Init(levelObject, token);
            if (paintableGroups != null && paintableGroups.Count > 0)
            {
                _paintableGroups.AddRange(paintableGroups);
            }

            zPos += SortingStepZ;
        }

        _levelObjectsControllers.ForEach(x => x.SetColors());
    }

    public void OpenLevelObjects(bool isOpen)
    {
        _levelObjectsControllers.ForEach(x => x.OpenObjects(isOpen));
    }

    public Sequence PlaceLevelObjects()
    {
        var levelObjectControllers = new List<LevelObjectController>();
        foreach (var loController in _levelObjectsControllers)
        {
            levelObjectControllers.AddRange(loController.LevelObjectControllers);
        }
        levelObjectControllers = levelObjectControllers.OrderBy(x => x.transform.localPosition.x).ToList();

        var tweenDuration = .4f;
        var tweenShift = .1f;

        var sequence = DOTween.Sequence();
        if (levelObjectControllers != null)
        {
            for (int i = 0; i < levelObjectControllers.Count; i++)
            {
                sequence.Insert(tweenShift * i, levelObjectControllers[i].PlaceObjectAnimation(tweenDuration));
            }
        }
        return sequence;
    }

    //reset variables, instantiate sprites container if null, assign in zoom, set base view settings
    public void SetDefaults()
    {
        _lastClickedGroup = null;
        _pointerDownTime = 0;
        _firstPaintedGroupsCount = 0;

        if (_objectsContainer == null)
        {
            _objectsContainer = Instantiate(_levelObjectsContainerPrefab, transform);
        }
        else if (_levelObjectsControllers != null && _levelObjectsControllers.Count > 0)
        {
            _levelObjectsControllers.ForEach(x => {
                x.OpenObjects(false);
                x.SetColors();
            });
            _paintableGroups.ForEach(x =>
            {
                x.IsFirstPainted = false;
                x.SetActiveOriginalStroke(true);
                x.SetFillColor(x.OriginalFillColor);
            });
        }

        _zoom.AssignZoomContainer(_objectsContainer);

        SpriteMaskEnabled = false;
        ZoomEnabled = false;
    }

    //destroy sprites container
    public void DestroyVectorSprites()
    {
        if (_objectsContainer == null)
        {
            return;
        }
        Destroy(_objectsContainer.gameObject);
        _objectsContainer = null;
        _levelObjectsControllers = new List<LevelObjectsController>();
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
            x.SetFillColor(_levelObjectsSettings.ClearFillColor);
            x.SetStrokeColor(_levelObjectsSettings.HighlightedStrokeColor);
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
                paintableSpriteGroup.SetStrokeColor(_levelObjectsSettings.RightStrokeColor);
                rightCount++;
            }
            else
            {
                paintableSpriteGroup.SetStrokeColor(_levelObjectsSettings.WrongStrokeColor);
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
        var vectorSpriteWidth = _levelObjectsSettings.SceneRect.width / _levelObjectsSettings.PixelsPerUnit;
        var scaleFactor = width / vectorSpriteWidth;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }
}