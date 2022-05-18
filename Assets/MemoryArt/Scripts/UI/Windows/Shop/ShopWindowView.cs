using System;
using Dainty.UI.WindowBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryArt.UI.Windows
{
    public class ShopWindowView : AWindowView
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundShadeButton;

        [Space] [SerializeField] private Button _freeCoinsButton;
        [SerializeField] private TMP_Text _freeCoinsAmountText;

        public event Action CloseButtonClick;
        public event Action FreeCoinsClick;

        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            _closeButton.onClick.AddListener(OnCloseClicked);
            _backgroundShadeButton.onClick.AddListener(OnCloseClicked);

            _freeCoinsButton.onClick.AddListener(() => FreeCoinsClick?.Invoke());
        }

        protected override void OnUnSubscribe()
        {
            base.OnSubscribe();
            _backgroundShadeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();

            _freeCoinsButton.onClick.RemoveAllListeners();
        }

        private void OnCloseClicked()
        {
            CloseButtonClick?.Invoke();
        }

        public void SetFreeCoinsAmount(int freeCoinsAmount)
        {
            _freeCoinsAmountText.text = freeCoinsAmount.ToString();
        }
    }
}