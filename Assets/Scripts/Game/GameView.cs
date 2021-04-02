using System;
using System.Threading.Tasks;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameView : AWindowView
{
    public event Action<PassedLevelStats> OnLevelDone; 

    [SerializeField] private PlayableObjectsController _playableControllerPrefab;
    [SerializeField] private SwatchesController _colorsController;
    [SerializeField] private Button _startButton;
    [SerializeField] private RectTransform _imageZone;
    [SerializeField] private LayoutElement _imageContainerLayoutElement;
    [SerializeField] private CounterElement _counterElement;

    private PlayableObjectsController _playableObjectsController;
    private GameSettings _gameSettings;
    private Sequence _startGameAnimation;
    private string _seconds = "s";
    private string _percents = "%";
    private string _of = "of";
    private string _done = "Done";
    private bool _imageZoneActiveSelf
    {
        set => _imageZone.gameObject.SetActive(value);
    }

    //init _gameSettings, instantiate playableController, update Image Zone
    void Awake()
    {
        _gameSettings = Settings.Instance.GameSettings;

        _playableObjectsController = Instantiate(_playableControllerPrefab, null);

        //UpdateImageZone - waiting before layout initialized
        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            var height = _imageZone.rect.height;
            _imageContainerLayoutElement.minHeight = height;
            _imageContainerLayoutElement.preferredHeight = height;
        });
    }

    protected override void OnSubscribe()
    {
        base.OnSubscribe();
        _playableObjectsController.OnFirstPaintedCountChanged += OnOnFirstClickedCountChanged;
        _playableObjectsController.OnPaintableSpriteClicked += OnPaintableClicked;
        _counterElement.OnButtonClicked += OnLevelDoneClicked;
        _startButton.onClick.AddListener(StartGame);
    }

    protected override void OnUnSubscribe()
    {
        base.OnUnSubscribe();
        _startGameAnimation?.Kill();
        _playableObjectsController.OnFirstPaintedCountChanged -= OnOnFirstClickedCountChanged;
        _playableObjectsController.OnPaintableSpriteClicked -= OnPaintableClicked;
        _counterElement.OnButtonClicked -= OnLevelDoneClicked;
        _startButton.onClick.RemoveListener(StartGame);
    }

    //load level by sending objects into playable objects controller, SetDefaults()
    public async Task InitLevel(Level levelAsset)
    {
        SetDefaults();
        foreach (var levelObject in levelAsset.LevelObjects)
        {
            //check copies count in object
            if (levelObject.CopiesSettings == null || levelObject.CopiesSettings.Count == 0)
            {
                Debug.LogError("No copies in "+ levelObject.SvgTextAsset.name);
                continue;
            }

            await _playableObjectsController.LoadLevelObject(levelObject);
        }

        _playableObjectsController.BlockingSpriteEnabled = true;
    }

    //reset playable controller, reset colors, reset counter, show play btn zone, close swatches
    public void SetDefaults(bool resetPaintableColors = false)
    {
        _playableObjectsController.SetDefaults();
        if (resetPaintableColors)
        {
            _playableObjectsController.SetOriginalColors();
        }

        //init timer
        _counterElement.SetDefaults();
        _counterElement.SetText($"{_gameSettings.TimerSeconds}{_seconds}");
        _counterElement.SetOutlineColor(_gameSettings.TimerColor);
        _counterElement.SetAmount(1);

        _imageZoneActiveSelf = true;

        _colorsController.Close();
    }

    public void DestroyLevel()
    {
        _playableObjectsController.DestroyVectorSprites();
    }

    private void StartGame()
    {
        //show image
        _playableObjectsController.BlockingSpriteEnabled = false;
        _imageZoneActiveSelf = false;

        //init timer
        int time = _gameSettings.TimerSeconds;

        _startGameAnimation = DOTween.Sequence();
        for (int i = 0; i < time; i++)
        {
            var currSeconds = time - i - 1;
            _startGameAnimation.AppendInterval(1f).AppendCallback(() =>
            {
                _counterElement.SetText($"{currSeconds}{_seconds}");
            });
        }

        _startGameAnimation
            .AppendCallback(() => _playableObjectsController.BlockingSpriteEnabled = true)
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                _colorsController.AddColors(_playableObjectsController.ClearColors());
                _playableObjectsController.BlockingSpriteEnabled = false;
                _colorsController.Show();

                _counterElement.SetText($"0 {_of} {_playableObjectsController.PaintablesCount}");
                _counterElement.SetAmount(0);
                _counterElement.SetOutlineColor(_gameSettings.ProgressColor);

                _playableObjectsController.ZoomEnabled = true;
            });

        _startGameAnimation.Play();

        //animating timer outline
        _counterElement.SetAmount(0, time);
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

    private void OnOnFirstClickedCountChanged(int updatedCount, int count)
    {
        var percents = (float)updatedCount / count;
        _counterElement.SetText($"{updatedCount} {_of} {count}");
        _counterElement.SetAmount(percents, .5f, Ease.InSine);
        if (updatedCount == count)
        {
            _counterElement.TransformToButton(_done);
        }
    }

    private void OnLevelDoneClicked()
    {
        OnLevelDone?.Invoke(_playableObjectsController.CheckSprite());
    }
}