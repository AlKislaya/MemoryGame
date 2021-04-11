using System;
using UnityEngine;

namespace Dainty.UI.WindowBase
{
    public abstract class AWindowView : MonoBehaviour
    {
        protected UiRoot UiRoot;
        protected Canvas Canvas;
        protected AWindowAnimation Animation;

        public void Initialize(UiRoot uiRoot)
        {
            UiRoot = uiRoot;

            Canvas = GetComponent<Canvas>();
            Animation = GetComponent<AWindowAnimation>();
            if (Animation != null)
            {
                Animation.Initialize(uiRoot);
            }

            OnInitialized();
        }

        public void Subscribe()
        {
            OnSubscribe();
        }

        public void UnSubscribe()
        {
            OnUnSubscribe();
        }

        public virtual void Show(bool push = false, bool animation = true, Action animationFinished = null)
        {
            Canvas.enabled = true;

            if (Animation != null)
            {
                if (animation)
                {
                    Animation.PlayShowAnimation(push, animationFinished);
                }
                else
                {
                    Animation.ShowImmediate();
                    animationFinished?.Invoke();
                }
            }
            else
            {
                animationFinished?.Invoke();
            }
        }

        public virtual void Close(bool pop, bool animation = true, Action animationFinished = null)
        {
            if (Animation != null)
            {
                if (animation)
                {
                    Animation.PlayCloseAnimation(pop, () =>
                    {
                        Canvas.enabled = false;
                        animationFinished?.Invoke();
                    });
                }
                else
                {
                    Animation.CloseImmediate();
                    {
                        Canvas.enabled = false;
                        animationFinished?.Invoke();
                    }
                }
            }
            else
            {
                Canvas.enabled = false;
                animationFinished?.Invoke();
            }
        }

        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnSubscribe()
        {
        }

        protected virtual void OnUnSubscribe()
        {
        }
    }
}