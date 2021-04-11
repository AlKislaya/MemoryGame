using Dainty.UI;
using Dainty.UI.WindowBase;

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
        base.BeforeShow();
        ApplicationController.Instance.TopPanelController.Close();
    }

    protected override void OnSubscribe()
    {
        view.OnLevelsTypeClicked += OnLevelTypeClicked;
    }

    protected override void OnUnSubscribe()
    {
        view.OnLevelsTypeClicked -= OnLevelTypeClicked;
    }

    private void OnLevelTypeClicked(string key)
    {
        if (key == _levelsManager.BaseLevelsKey)
        {
            ApplicationController.Instance.UiManager.Open<LevelsSequenceController, LevelsSequenceSettings>
                (new LevelsSequenceSettings() { Category = _levelsManager.GetCategoryByKey(key) },
                false,
                WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
        }
        else
        {
            ApplicationController.Instance.UiManager.Open<CategoriesSequenceController>(false, WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
        }
    }
}
