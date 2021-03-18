using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextAsset _svgLevelAsset;
    [SerializeField] private PlayableVectorSpritesController _playableSpriteController;
    [SerializeField] private SpriteRenderer _blockingSprite;
    [SerializeField] private Button _startButton;
    [SerializeField] private RectTransform _imageZone;
    [SerializeField] private LayoutElement _imageContainerLayoutElement;
    [SerializeField] private CounterElement _counterElement;

    private GameSettings _gameSettings;
    private string _seconds = "s";
    private string _percents = "%";
    private string _done = "Done";

    private void Awake()
    {
        _startButton.onClick.AddListener(StartGame);
        _gameSettings = Settings.Instance.GameSettings;

    }
    private void Start()
    {
        _playableSpriteController.LoadVectorSprite(_svgLevelAsset);
        _counterElement.SetText($"{_gameSettings.TimerSeconds}{_seconds}");
        StartCoroutine(UpdateImageZone());
    }

    private void StartGame()
    {
        _blockingSprite.enabled = false;
        _imageZone.gameObject.SetActive(false);
        StartCoroutine(StartTimer(_gameSettings.TimerSeconds));
    }

    private IEnumerator StartTimer(int time)
    {
        float slicedStep = 1f / time;
        _counterElement.SetOutlineColor(_gameSettings.TimerColor);
        _counterElement.SetAmount(1);

        for (int i = 1; i <= time; i++)
        {
            yield return new WaitForSeconds(1);
            _counterElement.SetText($"{time - i}{_seconds}");
            _counterElement.SetAmount(_counterElement.FillAmount- slicedStep);
        }

        _blockingSprite.enabled = true;
        yield return new WaitForSeconds(1);
        _playableSpriteController.ClearColors();
        yield return new WaitForSeconds(1);
        _playableSpriteController.ShowSwatches();
        _blockingSprite.gameObject.SetActive(false);
        _counterElement.SetText($"0{_percents}");
        _counterElement.SetAmount(0);
        _counterElement.SetOutlineColor(_gameSettings.ProgressColor);

        _playableSpriteController.OnFirstClickedCountChanged += OnOnFirstClickedCountChanged;
    }
    
    private void OnOnFirstClickedCountChanged(int updatedCount, int count)
    {
        var percents = (float) updatedCount / count;
        _counterElement.SetText($"{(int)(percents * 100)}{_percents}");
        _counterElement.SetAmount(percents);
        if (updatedCount == count)
        {
            _counterElement.TransformToButton(_done, OnLevelDoneClicked);
        }
    }

    private void OnLevelDoneClicked()
    {
        Debug.Log(_playableSpriteController.CheckSprite());
    }

    private IEnumerator UpdateImageZone()
    {
        yield return new WaitForSeconds(1);

        var height = _imageZone.rect.height;
        _imageContainerLayoutElement.minHeight = height;
        _imageContainerLayoutElement.preferredHeight = height;
    }
}