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
    [SerializeField] private RectTransform _staticCardDefaultParent;
    [SerializeField] private List<RectTransform> _cardsContaines;
    [SerializeField] private GameObject _selectableParent;
    [SerializeField] private List<GameObject> _selectableCardsParents;
    [SerializeField] private List<SelectableCard> _selectableCards;

    private Vector3 _placeAnimationScale = new Vector3(1.1f, 1.1f, 1f);
    private const float _checkCardAnimDuration = .7f;

    private SvgLoader _svgLoader = new SvgLoader();
    private List<VectorImage> _vectorImages = new List<VectorImage>();
    private Dictionary<int, RoundStats> _roundStats;
    private int _lastClickedSelectable;
    private int _roundIndex;
    private int _maxSelectablesCount;

    private class RoundStats
    {
        public int RightIndex;
        public int SelectedIndex;
    }

    private void Start()
    {
        InitSelectables();
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
        _maxSelectablesCount = _vectorImages.Max(x =>
        {
            if (x is SelectableImages selectable)
            {
                return selectable.Children.Count;
            }
            else
            {
                return 0;
            }
        });
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
                _staticCard.AddImage(staticSprite, i);

                for (int j = 0; j < _maxSelectablesCount; j++)
                {
                    _selectableCards[j].AddImage(staticSprite, i);
                }
            }
        }
    }

    public void SetDefaults()
    {
        OpenCard(false);
        _selectableCards.ForEach(x => x.ResetCard());

        _selectableParent.SetActive(false);
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

        _staticCard.SetParent(_staticCardDefaultParent);
        _staticCard.DoStretch();
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
        DOTween.Sequence().Append(_staticCard.DoHideMove(.5f)).AppendCallback(() => { _staticCard.SetActive(false); });

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

        _lastClickedSelectable = selectableIndex;
        _roundStats[_roundIndex].SelectedIndex = selectableIndex;
        var selectedImage = (_vectorImages[_roundIndex] as SelectableImages).Children[selectableIndex];

        for (int i  = 0; i < _maxSelectablesCount; i++)
        {
            _selectableCards[i].AddImage(selectedImage, _roundIndex);
        }

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
            _selectableCards[i].SetActive(true);
            _selectableCards[i].AddImage(sprites[i], _roundIndex);
        }

        for (int i = sprites.Count; i < _selectableCards.Count; i++)
        {
            _selectableCards[i].SetActive(false);
        }

        _selectableCardsParents[1].SetActive(sprites.Count >= 3);
    }

    private void InitSelectables()
    {
        for (int i = 0; i < _selectableCards.Count; i++)
        {
            _selectableCards[i].Index = i;
            _selectableCards[i].OnButtonClicked += OnSelectableClicked;
        }
    }
    //private void InitSelectables(int count)
    //{
    //    for (int i = _selectableCardsInstances.Count; i < count; i++)
    //    {
    //        var newSelectable = Instantiate(_selectableCardPrefab, _selectableParent.transform);
    //        _selectableCardsInstances.Add(newSelectable);
    //        newSelectable.Index = i;
    //        newSelectable.OnButtonClicked += OnSelectableClicked;
    //    }
    //}

    public Sequence CheckCardAnimation()
    {
        var sequence = DOTween.Sequence();
        for (int i = 0; i < _selectableCards.Count; i++)
        {
            if (i != _lastClickedSelectable)
            {
                sequence.Join(_selectableCards[i].DoFade(0, _checkCardAnimDuration));
            }
            else
            {
                _selectableCards[i].SetParent(_cardsContaines[1]);

                sequence
                    .Join(_selectableCards[i].DoStretch(_checkCardAnimDuration))
                    .Join(_selectableCards[i].DoMove(_checkCardAnimDuration));
            }
        }

        sequence.AppendCallback(() =>
        {
            _staticCard.SetActive(true);
            _staticCard.OpenCard(true);
            _staticCard.SetParent(_cardsContaines[0]);
            _staticCard.DoStretch();
        });

        return sequence;
    }
}