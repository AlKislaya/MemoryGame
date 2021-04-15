using System;
using System.Collections.Generic;
using System.Linq;
using Dainty.UI.WindowBase;
using UnityEngine;

public class CategoriesSequenceView : AWindowView
{
    [SerializeField] private CategoryItem _categoryPrefab;
    [SerializeField] private Transform _categoriesContainer;
    private List<CategoryItem> _categoriesInstances = new List<CategoryItem>();
    public event Action<LevelsCategory, bool> OnCategoryClicked;

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
        newCategoryInstance.Initialize(categoryData, onCategoryClicked);

        newCategoryInstance.SetAvailableState(isOpened);
        newCategoryInstance.SetPassedLevels(passedLevels);

        _categoriesInstances.Add(newCategoryInstance);
    }

    private void onCategoryClicked(LevelsCategory data, bool isOpened)
    {
        OnCategoryClicked?.Invoke(data, isOpened);
    }
}