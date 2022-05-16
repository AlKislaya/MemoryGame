using Dainty.UI;
using Dainty.UI.WindowBase;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LocalizationModule;

public class CategoriesSequenceController : AWindowController<CategoriesSequenceView>
{
    private const string HeaderKey = "categories_sequence";
    private const string CancelKey = "cancel";
    private const string NotEnoughtMoney_HeaderKey = "alert_not_enought_money_header";
    private const string NotEnoughtMoney_TextKey = "alert_not_enought_money_text";
    private const string GoToShopKey = "go_to_shop";
    private const string EnoughtMoney_HeaderKey = "alert_enought_money_header";
    private const string EnoughtMoney_TextKey = "alert_enought_money_text";
    private const string BuyKey = "buy";
    private string _header;
    public override string WindowId { get; }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        _header = Localization.Instance.GetLocalByKey(HeaderKey);
    }

    public override void BeforeShow()
    {
        base.BeforeShow();
        ApplicationController.Instance.TopPanelController.Show(_header);
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
            var localization = Localization.Instance;
            if (category.Price > MoneyController.Instance.MoneyBalance)
            {
                uiManager.Open<AlertController, AlertSettings>(
                    new AlertSettings()
                    {
                        HeaderText = localization.GetLocalByKey(NotEnoughtMoney_HeaderKey),
                        DialogText = localization.GetLocalByKey(NotEnoughtMoney_TextKey),
                        OnBackButtonClicked = Back,
                        Buttons = new List<AlertButtonSettings>(){ new AlertButtonSettings()
                        {
                            Callback = Back,
                            Text = localization.GetLocalByKey(CancelKey),
                            Color = AlertButtonColor.White
                        },
                        new AlertButtonSettings()
                        {
                            Callback = () =>
                            {
                                Back();
                                uiManager.Open<ShopController>(true);
                            },
                            Text = localization.GetLocalByKey(GoToShopKey),
                            Color = AlertButtonColor.Green
                        }
                        }
                    }, true);

            }
            else
            {
                uiManager.Open<AlertController, AlertSettings>(
                    new AlertSettings()
                    {
                        HeaderText = localization.GetLocalByKey(EnoughtMoney_HeaderKey),
                        DialogText = localization.GetLocalByKey(EnoughtMoney_TextKey),
                        OnBackButtonClicked = Back,
                        Buttons = new List<AlertButtonSettings>(){ new AlertButtonSettings()
                        {
                            Callback = Back,
                            Text = localization.GetLocalByKey(CancelKey),
                            Color = AlertButtonColor.White
                        },
                        new AlertButtonSettings()
                        {
                            Callback = () =>
                            {
                                Back();
                                if (MoneyController.Instance.GetMoney(category.Price))
                                {
                                    LevelsManager.Instance.GetLevelsProgressByCategory(category.Key);
                                    view.CreateOrUpdateCategory(category, true, 0);
                                }
                            },
                            Text = localization.GetLocalByKey(BuyKey),
                            Color = AlertButtonColor.Green
                        }
                        }
                    }, true);
            }
            return;
        }

        uiManager.Open<LevelsSequenceController, LevelsSequenceSettings>
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

    private void Back()
    {
        uiManager.Back();
    }

    protected override void OnEscape()
    {
        uiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }
}
