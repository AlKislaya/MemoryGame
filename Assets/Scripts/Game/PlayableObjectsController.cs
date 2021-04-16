using System;
using System.Collections.Generic;
using System.Linq;
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

    private const float RayDistance = 5f;
    private const float ClickDelay = .3f;

    [SerializeField] private LevelObjectController _levelObjectPrefab;
    [SerializeField] private Transform _levelObjectsContainerPrefab;
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private SpriteMask _spriteMask;
    [SerializeField] private Zoom _zoom;

    private SvgLoader _svgLoader;
    private Camera _cameraMain;
    private Transform _objectsContainer;
    private ContactFilter2D _contactFilter;
    private VectorSpriteSettings _levelObjectsSettings;
    private List<LevelObjectController> _levelObjects = new List<LevelObjectController>();
    private List<PaintableSpriteGroup> _paintableGroups = new List<PaintableSpriteGroup>();

    //
    private PaintableSpriteGroup _lastClickedGroup;
    private float _pointerDownTime;
    private int _firstPaintedGroupsCount;
    private float _levelObjPosZ;
    //

    //assign non-changeable variables, fit in screen size
    private void Awake()
    {
        _levelObjectsSettings = Settings.Instance.VectorSpriteSettings;
        _svgLoader = new SvgLoader(_levelObjectsSettings);
        _cameraMain = Camera.main;
        _contactFilter = new ContactFilter2D() { layerMask = _raycastMask, useLayerMask = true };
        FitInScreenSize();
    }

    public async Task LoadLevelObject(LevelObject levelObject)
    {
        _svgLoader.ImportSVG(levelObject.SvgTextAsset);

        //Debug.Log("LOADING STARTED "+ Time.time);
        //tesselate and build sprites
        List<SvgLoader.VectorSprite> vectorSprites = new List<SvgLoader.VectorSprite>();
        if (levelObject.IsStatic)
        {
            var staticSprite = await _svgLoader.GetStaticSprite();
            vectorSprites.Add(staticSprite);
        }
        else
        {
            vectorSprites = await _svgLoader.GetSpritesArrange();
        }
        //Debug.Log("LOADING ENDED " + Time.time);

        if (vectorSprites.Count == 0)
        {
            Debug.LogError("0 sprites");
            return;
        }


        for (int i = 0; i < levelObject.CopiesSettings.Count; i++)
        {
            //means that level loading canceled and objects container has been destroyed
            if (_objectsContainer == null)
            {
                Debug.LogError("_objectsContainer == null");
                return;
            }
            _levelObjPosZ += -.1f;
            var objectInstance = Instantiate(_levelObjectPrefab, _objectsContainer);

            objectInstance.Init(vectorSprites);
            objectInstance.InitSettings(levelObject.CopiesSettings[i], _levelObjPosZ);

            if (!levelObject.IsStatic)
            {
                _paintableGroups.AddRange(objectInstance.PaintableSpriteGroups);
                _levelObjects.Add(objectInstance);
            }
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

    public Sequence OpenLevelObjects(bool isOpen)
    {
        var tweenDuration = .5f;
        var tweenShift = .1f;

        var sequence = DOTween.Sequence();
        if (_levelObjects != null)
        {
            for (int i = 0; i < _levelObjects.Count; i++)
            {
                int storedI = i;
                sequence.AppendCallback(() => _levelObjects[storedI].OpenObject(isOpen));
                //sequence.Insert(tweenShift * i, _levelObjects[i].OpenObjectAnimation(tweenDuration, isOpen));
            }
        }
        return sequence;
    }

    public Sequence PlaceLevelObjects()
    {
        _levelObjects = _levelObjects.OrderBy(x=>x.transform.localPosition.x).ToList();

        var tweenDuration = .5f;
        var tweenShift = .3f;

        var sequence = DOTween.Sequence();
        if (_levelObjects != null)
        {
            for (int i = 0; i < _levelObjects.Count; i++)
            {
                sequence.Insert(tweenShift * i, _levelObjects[i].PlaceObjectAnimation(tweenDuration));
            }
        }
        return sequence;
    }

    //reset variables, instantiate sprites container if null, assign in zoom, set base view settings
    public void SetDefaults()
    {
        _levelObjPosZ = 0;
        _lastClickedGroup = null;
        _pointerDownTime = 0;
        _firstPaintedGroupsCount = 0;

        if (_objectsContainer == null)
        {
            _objectsContainer = Instantiate(_levelObjectsContainerPrefab, transform);
        }
        else if (_levelObjects != null && _levelObjects.Count > 0)
        {
            _levelObjects.ForEach(x => x.OpenObject(false));
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

    //public void SetOriginalColors()
    //{
    //    _paintableGroups.ForEach(x =>
    //    {
    //        x.SetActiveOriginalStroke(true);
    //        x.SetFillColor(x.OriginalFillColor);
    //    });
    //}

    //destroy sprites container
    public void DestroyVectorSprites()
    {
        if (_objectsContainer == null)
        {
            return;
        }
        Destroy(_objectsContainer.gameObject);
        _objectsContainer = null;
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