using System;
using Dainty.UI.Interfaces;
using UnityEngine;

namespace Dainty.UI.WindowBase
{
    public abstract class AWindowController<T> : IWindowController where T : AWindowView
    {
        protected IUiManager uiManager;
        protected T view;

        public Transform ViewTransform => view != null ? view.transform : null;
        public Type ViewType => typeof(T);
        public abstract string WindowId { get; }

        public void Initialize(IUiManager uiManager, AWindowView view)
        {
            this.uiManager = uiManager;
            this.view = (T) view;

            OnInitialize();
        }

        public virtual void OnOpened()
        {
        }

        public virtual void Dispose()
        {
        }

        public void Subscribe()
        {
            view.Subscribe();
            uiManager.Escape += OnEscape;

            OnSubscribe();
        }

        public void UnSubscribe()
        {
            OnUnSubscribe();

            uiManager.Escape -= OnEscape;
            view.UnSubscribe();
        }

        public virtual void BeforeShow()
        {
        }

        public virtual void Show(bool push, bool animation = true, Action animationFinished = null)
        {
            view.Show(push, animation, animationFinished);
        }

        public virtual void Close(bool pop, bool animation = true, Action animationFinished = null)
        {
            view.Close(pop, animation, animationFinished);
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnSubscribe()
        {
        }

        protected virtual void OnUnSubscribe()
        {
        }

        protected virtual void OnEscape()
        {
            if (uiManager.WindowsCount > 1)
            {
                uiManager.Back();
            }
        }
    }
}