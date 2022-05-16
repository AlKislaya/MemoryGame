using Dainty.UI.WindowBase;

public class ShopController : AWindowController<ShopView>
{
    public override string WindowId { get; }

    protected override void OnSubscribe()
    {
        view.CloseButtonClick += ViewOnCloseButtonClick;
    }

    protected override void OnUnSubscribe()
    {
        view.CloseButtonClick -= ViewOnCloseButtonClick;
    }

    private void ViewOnCloseButtonClick()
    {
        uiManager.Back();
    }
}
