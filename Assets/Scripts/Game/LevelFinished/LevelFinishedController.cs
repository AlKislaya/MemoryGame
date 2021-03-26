using Dainty.UI.WindowBase;

public class LevelFinishedController : AWindowController<LevelFinishedView>
{
    public override string WindowId { get; }

    protected override void OnSubscribe()
    {
        view.OnMenuButtonClicked += ViewOnOnMenuButtonClicked;
        view.OnPlayButtonClicked += ViewOnOnPlayButtonClicked;
        view.OnReplayButtonClicked += ViewOnOnReplayButtonClicked;
    }
    protected override void OnUnSubscribe()
    {
        view.OnMenuButtonClicked -= ViewOnOnMenuButtonClicked;
        view.OnPlayButtonClicked -= ViewOnOnPlayButtonClicked;
        view.OnReplayButtonClicked -= ViewOnOnReplayButtonClicked;
    }

    private void ViewOnOnReplayButtonClicked()
    {
        
    }

    private void ViewOnOnPlayButtonClicked()
    {

    }

    private void ViewOnOnMenuButtonClicked()
    {
        var manager = ApplicationController.Instance.UiManager;
        manager.Back();
        manager.Back();
    }
}
