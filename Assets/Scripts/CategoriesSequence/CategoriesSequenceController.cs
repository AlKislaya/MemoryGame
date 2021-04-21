using Dainty.UI;
using Dainty.UI.WindowBase;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CategoriesSequenceController : AWindowController<CategoriesSequenceView>
{
    private const string  Header = "Categories";
    public override string WindowId { get; }

    public override void BeforeShow()
    {
        base.BeforeShow();
        ApplicationController.Instance.TopPanelController.Show(Header);
        var levelsManager = LevelsManager.Instance;
        var categories = levelsManager.LevelsCategories;

        for (int i = 0; i < categories.Count; i++)
        {
            var progressExists = levelsManager.IsCategoryProgressExists(categories[i].Key);
            var passedLevelsCount = progressExists ?
                levelsManager.GetLevelsProgressByCategory(categories[i].Key).Levels.Count(x => x.IsPassed) : 0;

            view.CreateOrUpdateCategory(categories[i], categories[i].Price == 0 || progressExists, passedLevelsCount);
        }
    }

    private void OnCategoryClicked(LevelsCategory category, bool isOpened)
    {
        if (!isOpened)
        {
            ApplicationController.Instance.UiManager.Open<AlertController, AlertSettings>(
                new AlertSettings()
                {
                    HeaderText = "Hey",
                    DialogText = "duuuuude",
                    Buttons = new List<AlertButtonSettings>(){ new AlertButtonSettings()
                    {
                        Callback = () => ApplicationController.Instance.UiManager.Back(),
                        Text = "give 5 dollars",
                        Color = Color.green
                    }
                    }
                }, true);
            //view.CreateOrUpdateCategory(category, true, 0);
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

    protected override void OnEscape()
    {
        ApplicationController.Instance.UiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }
}
