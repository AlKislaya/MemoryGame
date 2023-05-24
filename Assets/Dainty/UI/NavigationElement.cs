using Dainty.UI.WindowBase;

namespace Dainty.UI
{
    public class NavigationElement
    {
        public readonly IWindowController WindowController;
        public readonly bool IsPopup;

        public NavigationElement(IWindowController windowController, bool isPopup)
        {
            WindowController = windowController;
            IsPopup = isPopup;
        }
    }
}