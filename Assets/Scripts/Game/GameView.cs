using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameView : AWindowView
{
    public event Action<PassedLevelStats> OnLevelDone;
    public event Action<bool> OnBlockExitStateChanged;
    public event Action OnHintClicked;
    public bool BlockExit => _checkLevelAnimation != null && _checkLevelAnimation.IsPlaying();

    [SerializeField] private ButtonAnimation _hintButton;
    [SerializeField] private PlayableObjectsController _playableControllerPrefab;
    [SerializeField] private SwatchesController _colorsController;
    [SerializeField] private List<Button> _tapToStartButtons;
    [SerializeField] private LayoutElement _imageContainerLayoutElement;
    [SerializeField] private CounterElement _counterElement;
    [SerializeField] private GameObject _loader;

    private PlayableObjectsController _playableObjectsController;
    private GameSettings _gameSettings;
    private Sequence _startGameAnimation;
    private Sequence _placeObjectsAnimation;
    private Sequence _checkLevelAnimation;

    private string _seconds = "s";
    private string _of = "of";
    private string _done = "Done";

    //init _gameSettings, instantiate playableController, update Image Zone
    //set playable as child
    void Awake()
    {
        _gameSettings = Settings.Instance.GameSettings;

        _playableObjectsController = Instantiate(_playableControllerPrefab, null);
        _playableObjectsController.transform.SetParent(transform);
        _playableObjectsController.transform.position = Vector3.zero;

        //UpdateImageZone - waiting before layout initialized
        DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
        {
            var height = _imageContainerLayoutElement.GetComponent<RectTransform>().sizeDelta.x;
            _imageContainerLayoutElement.minHeight = height;
            _imageContainerLayoutElement.preferredHeight = height;
        });
    }

    protected override void OnSubscribe()
    {
        base.OnSubscribe();

        _hintButton.Button.onClick.AddListener(OnHintButtonClicked);

        _playableObjectsController.OnFirstPaintedCountChanged += OnOnFirstClickedCountChanged;
        _playableObjectsController.OnPaintableSpriteClicked += OnPaintableClicked;
        _counterElement.OnButtonClicked += OnLevelDoneClicked;
        _tapToStartButtons.ForEach(x => x.onClick.AddListener(StartGame));
    }

    protected override void OnUnSubscribe()
    {
        base.OnUnSubscribe();
        _hintButton.Button.onClick.RemoveAllListeners();
        _playableObjectsController.OnFirstPaintedCountChanged -= OnOnFirstClickedCountChanged;
        _playableObjectsController.OnPaintableSpriteClicked -= OnPaintableClicked;
        _counterElement.OnButtonClicked -= OnLevelDoneClicked;
        _tapToStartButtons.ForEach(x => x.onClick.RemoveListener(StartGame));
    }

    public async Task InitLevel(Level levelAsset, CancellationToken token)
    {
        await _playableObjectsController.LoadLevel(levelAsset, token);
        _colorsController.AddColors(_playableObjectsController.GetColors());
    }

    public void ShowLoader(bool show)
    {
        _loader.SetActive(show);
    }

    //place objects animation
    public void PlaceObjects()
    {
        _placeObjectsAnimation = _playableObjectsController.PlaceLevelObjects();
        _placeObjectsAnimation.AppendCallback(() => _tapToStartButtons.ForEach(x => x.gameObject.SetActive(true)));

        _playableObjectsController.SpriteMaskEnabled = true;

        _placeObjectsAnimation.Play();
    }

    //reset playable controller, reset colors, reset counter, show play btn zone, close swatches
    public void SetDefaults()
    {
        _playableObjectsController.SetDefaults();
        //hide tap to start buttons
        _tapToStartButtons.ForEach(x => x.gameObject.SetActive(false));
        //init timer
        SetTimer();

        _colorsController.Close();

        if (_hintButton.Button != null)
        {
            _hintButton.Button.interactable = false;
            _hintButton.SetAnimationEnabled(false);
        }
    }

    private void SetTimer()
    {
        _counterElement.SetDefaults();
        _counterElement.SetText($"{_gameSettings.TimerSeconds}{_seconds}");
        _counterElement.SetColor(_gameSettings.TimerColor);
        _counterElement.SetAmount(1);
    }

    public void ShowHint()
    {
        _tapToStartButtons.ForEach(x => x.gameObject.SetActive(true));
        _playableObjectsController.ZoomEnabled = false;

        //
        SetTimer();

        _colorsController.Close();

        if (_hintButton.Button != null)
        {
            _hintButton.Button.interactable = false;
            _hintButton.SetAnimationEnabled(false);
        }
        //

        _playableObjectsController.StoreColors();
        _playableObjectsController.OpenLevelObjects(false);
        _playableObjectsController.SetOriginalColors();
    }

    public void DestroyLevel()
    {
        //if (delay == 0)
        //{
            _playableObjectsController.DestroyVectorSprites();
            //return;
        //}
        //_destroyPlayableImage = DOTween.Sequence()
        //    .AppendInterval(delay)
        //    .AppendCallback(() =>
        //    {
        //        Debug.Log("Destroy sprites");
        //        _playableObjectsController.DestroyVectorSprites();
        //    })
        //    .OnComplete(() => _destroyPlayableImage = null);
        //_destroyPlayableImage.Play();
    }

    public void StopAnimations()
    {
        _startGameAnimation?.Kill();
        _placeObjectsAnimation?.Kill();
    }

    private void StartGame()
    {
        _tapToStartButtons.ForEach(x => x.gameObject.SetActive(false));
        //show image
        _playableObjectsController.OpenLevelObjects(true);
        _startGameAnimation = DOTween.Sequence();

        //init timer
        int time = _gameSettings.TimerSeconds;

        _startGameAnimation
            .Append(_counterElement.TimerTween(time, _seconds))
            .AppendCallback(()=>_playableObjectsController.OpenLevelObjects(false))
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                _playableObjectsController.ClearColors();
                _playableObjectsController.OpenLevelObjects(true);
                _colorsController.Show();

                OnOnFirstClickedCountChanged(_playableObjectsController.FirstPaintedCount);
                _counterElement.SetColor(_gameSettings.ProgressColor);

                _playableObjectsController.ZoomEnabled = true;
                _hintButton.Button.interactable = true;
                _hintButton.SetAnimationEnabled(true);
            });

        _startGameAnimation.Play();
    }

    private void OnPaintableClicked()
    {
        var color = Color.white;
        if (!_colorsController.TryGetSelectedColor(ref color))
        {
            return;
        }

        _playableObjectsController.SetLastClickedGroupColor(color);
    }

    private void OnOnFirstClickedCountChanged(int updatedCount)
    {
        var count = _playableObjectsController.PaintablesCount;

        var percents = (float)updatedCount / count;
        _counterElement.SetText($"{updatedCount} {_of} {count}");
        _counterElement.SetAmount(percents, .5f, Ease.InSine);
        if (updatedCount == count)
        {
            _counterElement.TransformToButton(_done, _gameSettings.DoneColor);
        }
    }

    private void OnHintButtonClicked()
    {
        OnHintClicked?.Invoke();
    }

    private void OnLevelDoneClicked()
    {
        OnUnSubscribe();
        OnBlockExitStateChanged?.Invoke(true);
        _checkLevelAnimation =_playableObjectsController
            .CheckSpriteAnimation()
            .AppendInterval(.5f)
            .OnComplete(()=> 
            {
                OnBlockExitStateChanged?.Invoke(false);
                OnLevelDone?.Invoke(_playableObjectsController.GetStats());
            });
    }
}