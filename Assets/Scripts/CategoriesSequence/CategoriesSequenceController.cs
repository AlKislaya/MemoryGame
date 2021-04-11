using Dainty.UI;
using Dainty.UI.WindowBase;
using UnityEngine;

public class CategoriesSequenceController : AWindowController<CategoriesSequenceView>
{
    private const string  Header = "Categories";
    public override string WindowId { get; }
    protected override void OnInitialize()
    {
        base.OnInitialize();
        var levelsManager = LevelsManager.Instance;
        var categories = levelsManager.LevelsCategories;

        for (int i = 0; i < categories.Count; i++)
        {
            view.CreateOrUpdateCategory(categories[i], categories[i].Price == 0 || levelsManager.IsCategoryProgressExists(categories[i].Key));
        }
    }

    public override void BeforeShow()
    {
        base.BeforeShow();
        ApplicationController.Instance.TopPanelController.Show(Header);
    }

    private void OnCategoryClicked(LevelsCategory category, bool isOpened)
    {
        Debug.Log($"{category.Key} {isOpened}");
        if (!isOpened)
        {
            view.CreateOrUpdateCategory(category, true);
            return;
        }

        ApplicationController.Instance.UiManager.Open<LevelsSequenceController, LevelsSequenceSettings>
            (new LevelsSequenceSettings() {Category = category},
            false,
            WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
    }

    protected override void OnSubscribe()
    {
        view.OnCategoryClicked += OnCategoryClicked;
    }

    protected override void OnUnSubscribe()
    {
        view.OnCategoryClicked -= OnCategoryClicked;
    }
}
