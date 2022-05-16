using Dainty.UI;
using Dainty.UI.WindowBase;
using System;

public class MainController : AWindowController<MainView>
{
    public override string WindowId { get; }
    private LevelsManager _levelsManager;
    protected override void OnInitialize()
    {
        base.OnInitialize();
        _levelsManager = LevelsManager.Instance;
    }

    public override void BeforeShow()
    {
        ApplicationController.Instance.TopPanelController.Close();
        view.SetMoneyBalance(MoneyController.Instance.MoneyBalance);
    }

    protected override void OnSubscribe()
    {
        view.OnLevelsTypeClicked += OnLevelTypeClicked;
        view.OnSettingsClicked += OnSettingsClicked;
        view.OnShopClicked += OnShopClicked;
    }

    protected override void OnUnSubscribe()
    {
        view.OnLevelsTypeClicked -= OnLevelTypeClicked;
        view.OnSettingsClicked -= OnSettingsClicked;
        view.OnShopClicked -= OnShopClicked;
    }

    private void OnShopClicked()
    {
        uiManager.Open<ShopController>(true);
    }

    private void OnSettingsClicked()
    {
        uiManager.Open<SettingsController>(true);
    }

    private void OnLevelTypeClicked(string key)
    {
        if (key == _levelsManager.BaseLevelsKey)
        {
            uiManager.Open<LevelsSequenceController, LevelsSequenceSettings>
                (new LevelsSequenceSettings() { Category = _levelsManager.GetCategoryByKey(key) },
                false,
                WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
        }
        else
        {
            uiManager.Open<CategoriesSequenceController>(false, WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
        }
    }
}
