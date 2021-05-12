using Dainty.UI.WindowBase;

public class SettingsController : AWindowController<SettingsView>
{
    public override string WindowId { get; }

    public override void BeforeShow()
    {
        base.BeforeShow();
        view.SetDefaults();
    }
}
