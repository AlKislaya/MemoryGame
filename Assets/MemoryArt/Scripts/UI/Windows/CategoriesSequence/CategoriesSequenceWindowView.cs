using System;
using System.Collections.Generic;
using System.Linq;
using Dainty.UI.WindowBase;
using UnityEngine;

public class CategoriesSequenceWindowView : AWindowView
{
    [SerializeField] private CategoryItem _categoryPrefab;
    [SerializeField] private Transform _categoriesContainer;

    private readonly List<CategoryItem> _categoriesInstances = new List<CategoryItem>();

    public event Action<LevelsCategory, bool> CategoryClick;

    public void CreateOrUpdateCategory(LevelsCategory categoryData, bool isOpened, int passedLevels)
    {
        var category = _categoriesInstances.FirstOrDefault(x => x.Key.Equals(categoryData.Key));
        if (category != null)
        {
            if (isOpened != category.IsOpened)
            {
                category.SetAvailableState(isOpened);
            }

            category.SetPassedLevels(passedLevels);
            return;
        }

        var newCategoryInstance = Instantiate(_categoryPrefab, _categoriesContainer);
        newCategoryInstance.Initialize(categoryData, OnCategoryClick);

        newCategoryInstance.SetAvailableState(isOpened);
        newCategoryInstance.SetPassedLevels(passedLevels);

        _categoriesInstances.Add(newCategoryInstance);
    }

    private void OnCategoryClick(LevelsCategory data, bool isOpened)
    {
        CategoryClick?.Invoke(data, isOpened);
    }
}