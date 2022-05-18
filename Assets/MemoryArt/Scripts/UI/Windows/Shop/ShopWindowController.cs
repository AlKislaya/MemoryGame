using Dainty.UI.WindowBase;

namespace MemoryArt.UI.Windows
{
    public class ShopWindowController : AWindowController<ShopWindowView>
    {
        private const int FREE_COINS_AMOUNT = 10;

        public override string WindowId { get; }

        protected override void OnInitialize()
        {
            view.SetFreeCoinsAmount(FREE_COINS_AMOUNT);
        }

        protected override void OnSubscribe()
        {
            view.CloseButtonClick += ViewOnCloseButtonClick;
            view.FreeCoinsClick += ViewOnFreeCoinsClick;
        }

        protected override void OnUnSubscribe()
        {
            view.CloseButtonClick -= ViewOnCloseButtonClick;
        }

        private void ViewOnCloseButtonClick()
        {
            uiManager.Back();
        }

        private void ViewOnFreeCoinsClick()
        {
            ApplicationController.Instance.AdsController.ShowRewarded(null, success =>
            {
                if (success)
                {
                    MoneyController.Instance.AddMoney(FREE_COINS_AMOUNT);
                }
            }, null);
        }
    }
}