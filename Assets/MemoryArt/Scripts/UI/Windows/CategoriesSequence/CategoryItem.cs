using System;
using System.Collections.Generic;
using MemoryArt.Game.Levels;
using MemoryArt.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryArt.UI.Windows
{
    public class CategoryItem : MonoBehaviour
    {
        private const char Delimiter = '/';

        [SerializeField] private Button _button;
        [SerializeField] private LocalizableText _categoryName;
        [SerializeField] private TextMeshProUGUI _levelsCountText;
        [SerializeField] private GameObject _pricePanel;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Image _previewImage;
        [SerializeField] private List<GameObject> _lockElements;

        private LevelsCategory _category;
        private bool _isOpened;

        public bool IsOpened => _isOpened;
        public string Key => _category.Key;

        public void Initialize(LevelsCategory category, Action<LevelsCategory, bool> onButtonClickedCallback)
        {
            _category = category;

            _categoryName.Key = category.Key;
            _previewImage.sprite = category.Preview;
            _priceText.text = category.Price.ToString();

            _button.onClick.AddListener(() => onButtonClickedCallback(_category, _isOpened));
        }

        public void SetAvailableState(bool isOpened)
        {
            _isOpened = isOpened;
            _pricePanel.SetActive(!isOpened);
            _lockElements.ForEach(x => x.SetActive(!isOpened));
        }

        public void SetPassedLevels(int passedLevelsCount)
        {
            _levelsCountText.text = $"{passedLevelsCount}{Delimiter}{_category.LevelsSequence.Levels.Count}";
        }
    }
}