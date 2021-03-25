using System;
using System.Collections;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameView : AWindowView
{
    [SerializeField] private PlayableVectorSpritesController _playableControllerPrefab;
    [SerializeField] private SwatchesController _colorsController;
    [SerializeField] private Button _startButton;
    [SerializeField] private RectTransform _imageZone;
    [SerializeField] private LayoutElement _imageContainerLayoutElement;
    [SerializeField] private CounterElement _counterElement;

    private PlayableVectorSpritesController _playableSpriteController;
    private GameSettings _gameSettings;
    private string _seconds = "s";
    private string _percents = "%";
    private string _done = "Done";
    private bool _imageZoneActiveSelf
    {
        set => _imageZone.gameObject.SetActive(value);
    }

    //init _gameSettings, instantiate playableController, update Image Zone
    void Awake()
    {
        _gameSettings = Settings.Instance.GameSettings;

        _playableSpriteController = Instantiate(_playableControllerPrefab, null);

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
        _playableSpriteController.OnFirstPaintedCountChanged += OnOnFirstClickedCountChanged;
        _playableSpriteController.OnPaintableSpriteClicked += OnPaintableClicked;
        _counterElement.OnButtonClicked += OnLevelDoneClicked;
        _startButton.onClick.AddListener(StartGame);
    }

    protected override void OnUnSubscribe()
    {
        base.OnUnSubscribe();
        _playableSpriteController.OnFirstPaintedCountChanged -= OnOnFirstClickedCountChanged;
        _playableSpriteController.OnPaintableSpriteClicked -= OnPaintableClicked;
        _counterElement.OnButtonClicked -= OnLevelDoneClicked;
        _startButton.onClick.RemoveListener(StartGame);
    }

    //reset playable controller, load svg, reset counter, show play btn zone, close swatches
    public void InitGame(TextAsset svgLevelAsset)
    {
        _playableSpriteController.Reset();
        _playableSpriteController.LoadVectorSprite(svgLevelAsset);

        //init timer
        _counterElement.Reset();
        _counterElement.SetText($"{_gameSettings.TimerSeconds}{_seconds}");
        _counterElement.SetOutlineColor(_gameSettings.TimerColor);
        _counterElement.SetAmount(1);

        _imageZoneActiveSelf = true;

        _colorsController.Close();
    }

    public override void Close(bool animation = true, Action animationFinished = null)
    {
        base.Close(animation, animationFinished);
        _playableSpriteController.DestroyVectorSprites();
    }

    private void StartGame()
    {
        //show image
        _playableSpriteController.BlockingSpriteEnabled = false;
        _imageZoneActiveSelf = false;

        //init timer
        int time = _gameSettings.TimerSeconds;

        var sequence = DOTween.Sequence();
        for (int i = 0; i < time; i++)
        {
            var currSeconds = time - i - 1;
            sequence.AppendInterval(1f).AppendCallback(() =>
            {
                _counterElement.SetText($"{currSeconds}{_seconds}");
            });
        }

        sequence
            .AppendCallback(() => _playableSpriteController.BlockingSpriteEnabled = true)
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                _colorsController.AddColors(_playableSpriteController.ClearColors());
                _playableSpriteController.BlockingSpriteEnabled = false;
                _colorsController.Show();

                _counterElement.SetText($"0{_percents}");
                _counterElement.SetAmount(0);
                _counterElement.SetOutlineColor(_gameSettings.ProgressColor);

                _playableSpriteController.ZoomEnabled = true;
            });

        sequence.Play();

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

        _playableSpriteController.SetLastClickedGroupColor(color);
    }

    private void OnOnFirstClickedCountChanged(int updatedCount, int count)
    {
        var percents = (float)updatedCount / count;
        _counterElement.SetText($"{(int)(percents * 100)}{_percents}");
        _counterElement.SetAmount(percents, .5f, Ease.InSine);
        if (updatedCount == count)
        {
            _counterElement.TransformToButton(_done);
        }
    }

    private void OnLevelDoneClicked()
    {
        Debug.Log(_playableSpriteController.CheckSprite());
        LevelsManager.Instance.SetPassedLevel(LevelsManager.Instance.CurrentLevel, _playableSpriteController.CheckSprite());
        ApplicationController.Instance.UiManager.Close<GameController>();
    }
}