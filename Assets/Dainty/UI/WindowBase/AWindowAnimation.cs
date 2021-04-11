using System;
using UnityEngine;

namespace Dainty.UI.WindowBase
{
    public abstract class AWindowAnimation : MonoBehaviour
    {
        protected UiRoot UIRoot;

        private bool _initialized;

        public void Initialize(UiRoot uiRoot)
        {
            if (_initialized)
            {
                return;
            }

            UIRoot = uiRoot;
            OnInitialized();
            _initialized = true;
        }

        protected virtual void OnInitialized()
        {
        }

        public abstract void ShowImmediate();
        public abstract void PlayShowAnimation(bool push, Action animationFinished = null);

        public abstract void CloseImmediate();
        public abstract void PlayCloseAnimation(bool pop, Action animationFinished = null);
    }
}