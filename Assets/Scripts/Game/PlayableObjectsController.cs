using DG.Tweening;
using SvgLoaderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PassedLevelStats
{
    public int SelectableCount;
    public int RightSelectablesCount;
}

public class PlayableObjectsController : MonoBehaviour
{
    public event Action<PassedLevelStats> OnLevelEnded; 
    [SerializeField] private PlayableCard _staticCard;
    [SerializeField] private PlayableCard _playableCard;
    [SerializeField] private GameObject _selectableParent;
    [SerializeField] private SelectableCard _selectableCardPrefab;

    private Vector3 _placeAnimationScale = new Vector3(1.1f, 1.1f, 1f);

    private SvgLoader _svgLoader = new SvgLoader();
    private List<SelectableCard> _selectableCardsInstances = new List<SelectableCard>();
    private List<VectorImage> _vectorImages = new List<VectorImage>();
    private Dictionary<int, RoundStats> _roundStats;
    private int _roundIndex;

    private class RoundStats
    {
        public int RightIndex;
        public int SelectedIndex;
    }

    //loading svg
    public async Task LoadLevel(TextAsset levelAsset, CancellationToken token)
    {
        _svgLoader.ImportSVG(levelAsset);

        _vectorImages = await _svgLoader.GetSpritesArrange(token);

        if (_vectorImages == null || _vectorImages.Count == 0)
        {
            Debug.LogError("0 sprites");
            return;
        }
    }

    //set images, randomize selectables
    private void SetLevel()
    {
        _roundStats = new Dictionary<int, RoundStats>();

        for (int i = 0; i < _vectorImages.Count; i++)
        {
            if (_vectorImages[i] is SelectableImages selectableSprites)
            {
                int rightIndex = UnityEngine.Random.Range(0, selectableSprites.Children.Count);
                _roundStats.Add(i, new RoundStats()
                {
                    RightIndex = rightIndex
                });

                _staticCard.AddImage(selectableSprites.Children[rightIndex], i);
            }
            else
            {
                var staticSprite = (_vectorImages[i] as StaticVectorImage).Sprite;
                _playableCard.AddImage(staticSprite, i);
                _staticCard.AddImage(staticSprite, i);
            }
        }
    }

    public void SetDefaults()
    {
        _playableCard.ResetCard();
        OpenCard(false);

        _selectableParent.SetActive(false);
        _playableCard.SetActive(false);
        _staticCard.SetActive(false);
    }

    public void DestroyLevel()
    {
        _vectorImages = new List<VectorImage>();
        _staticCard.ResetCard();
    }

    public Tween PlaceCard(float duration)
    {
        SetLevel();

        _staticCard.SetActive(true);
        _staticCard.transform.localScale = _placeAnimationScale;
        return _staticCard.GetComponent<RectTransform>().DOScale(Vector3.one, duration);
    }

    public void OpenCard(bool isOpen)
    {
        _staticCard.OpenCard(isOpen);
    }

    public void StartGame()
    {
        _playableCard.SetActive(true);
        _staticCard.SetActive(false);

        _roundIndex = -1;
        _selectableParent.SetActive(true);
        ShowNextCards();
    }

    private void OnSelectableClicked(int selectableIndex)
    {
        if (_roundIndex >= _vectorImages.Count)
        {
            //GAME ENDED
            return;
        }

        _roundStats[_roundIndex].SelectedIndex = selectableIndex;

        _playableCard.AddImage((_vectorImages[_roundIndex] as SelectableImages).Children[selectableIndex], _roundIndex);
        ShowNextCards();
    }

    private void ShowNextCards()
    {
        _roundIndex++;
        if (_roundIndex >= _vectorImages.Count)
        {
            OnLevelEnded?.Invoke(new PassedLevelStats()
            {
                SelectableCount = _roundStats.Count,
                RightSelectablesCount = _roundStats.Count(x => x.Value.RightIndex == x.Value.SelectedIndex)
            }
            );
            return;
        }
        
        if (_vectorImages[_roundIndex] is StaticVectorImage)
        {
            ShowNextCards();
            return;
        }

        ShowSelectableCards();
    }

    private void ShowSelectableCards()
    {
        var sprites = (_vectorImages[_roundIndex] as SelectableImages).Children;
        for (int i = 0; i < sprites.Count; i++)
        {
            if (_selectableCardsInstances.Count <= i)
            {
                var newSelectable = Instantiate(_selectableCardPrefab, _selectableParent.transform);
                _selectableCardsInstances.Add(newSelectable);
                newSelectable.Index = i;
                newSelectable.OnButtonClicked += OnSelectableClicked;
            }
            _selectableCardsInstances[i].SetActive(true);
            _selectableCardsInstances[i].AddImage(sprites[i], 0);
        }

        for (int i = sprites.Count; i < _selectableCardsInstances.Count; i++)
        {
            _selectableCardsInstances[i].SetActive(false);
        }
    }

    public Sequence CheckCardAnimation()
    {
        return DOTween.Sequence().AppendCallback(() => 
        {
            _staticCard.SetActive(true);
            _selectableParent.SetActive(false);
        }
        ).AppendInterval(1f);
    }
}