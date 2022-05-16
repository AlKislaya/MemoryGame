using Dainty.UI.WindowBase;

public class SettingsController : AWindowController<SettingsView>
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
