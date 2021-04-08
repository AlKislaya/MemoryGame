using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryItem : MonoBehaviour
{
    public bool IsOpened => _isOpened;
    public string Key => _category.Key;

    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _categoryNameText;
    [SerializeField] private GameObject _pricePanel;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Image _previewImage;
    [SerializeField] private List<GameObject> _lockElements;
    private LevelsCategory _category;
    private bool _isOpened;

    public void Initialize(LevelsCategory category, Action<LevelsCategory, bool> onButtonClickedCallback)
    {
        _category = category;

        _categoryNameText.text = category.Key;
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
}