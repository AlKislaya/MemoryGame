using Dainty.UI.WindowBase;

public class SettingsWindowController : AWindowController<SettingsWindowView>
{
    public override string WindowId { get; }

    public override void BeforeShow()
    {
        base.BeforeShow();
        view.SetDefaults();
    }

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
