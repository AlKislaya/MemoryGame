using Dainty.UI.WindowBase;

namespace MemoryArt.UI.Windows
{
    public class ShopWindowController : AWindowController<ShopWindowView>
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
}