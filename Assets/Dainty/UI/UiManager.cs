using System;
using System.Linq;
using Dainty.UI.Interfaces;
using Dainty.UI.WindowBase;

namespace Dainty.UI
{
    public class UiManager : IUiManager
    {
        protected readonly UiManagerSettings _settings;
        protected readonly EscapeListener _escapeListener;
        protected readonly INavigationStack _navStack;
        protected readonly UiRoot _root;

        public UiManager(UiRoot root, UiManagerSettings settings) : this(root, settings, new NavigationStack())
        {
        }

        public UiManager(UiRoot root, UiManagerSettings settings, INavigationStack navigationStack)
        {
            _root = root;
            _settings = settings;
            _navStack = navigationStack;

            root.Destroying += RootOnDestroying;
            _escapeListener = EscapeListener.Instance;
            _escapeListener.Escape += EscapeListenerOnEscape;
        }

        public UiRoot UiRoot => _root;

        public IWindowController CurrentWindow
        {
            get
            {
                try
                {
                    return _navStack.Peek().WindowController;
                }
                catch
                {
                    return null;
                }
            }
        }

        public int WindowsCount => _navStack.Count;

        public event Action Escape;
        public event Action<IWindowController> WindowChanged;
        public event Action<IWindowController> WindowClosing;

        public bool IsOpened<T>()
        {
            return _navStack.IsOpened<T>();
        }

        #region Open

        public virtual T Open<T>(bool isPopup = false, WindowTransition transition = WindowTransition.AnimateOpening)
            where T : IWindowController, new()
        {
            var controller = OpenBase<T>();
            _navStack.Push(controller, transition, isPopup);
            WindowChanged?.Invoke(controller);
            controller.OnOpened();
            return controller;
        }

        public T Open<T>(WindowTransition transition)
            where T : IWindowController, new()
        {
            return Open<T>(false, transition);
        }

        public virtual T Open<T, TS>(TS data, bool isPopup,
            WindowTransition transition = WindowTransition.AnimateOpening)
            where T : IWindowController, IConfigurableWindow<TS>, new()
        {
            var controller = OpenBase<T>();
            controller.Initialize(data);

            _navStack.Push(controller, transition, isPopup);

            WindowChanged?.Invoke(controller);
            controller.OnOpened();
            return controller;
        }

        public T Open<T, TS>(TS data, WindowTransition transition = WindowTransition.AnimateOpening)
            where T : IWindowController, IConfigurableWindow<TS>, new()
        {
            return Open<T, TS>(data, false, transition);
        }

        #endregion

        #region Close

        public virtual bool Close<T>(WindowTransition transition = WindowTransition.AnimateClosing,
            Action onClosed = null)
        {
            WindowClosing?.Invoke(_navStack.Peek().WindowController);

            return _navStack.Close<T>(transition, out _, onClosed);
        }

        public bool Close<T>(Action onClosed)
        {
            return Close<T>(WindowTransition.AnimateClosing, onClosed);
        }

        public virtual bool Close<T>(out IWindowController window,
            WindowTransition transition = WindowTransition.AnimateClosing, Action onClosed = null)
        {
            WindowClosing?.Invoke(_navStack.Peek().WindowController);

            return _navStack.Close<T>(transition, out window, onClosed);
        }

        public bool Close<T>(out IWindowController window, Action onClosed)
        {
            return Close<T>(out window, WindowTransition.AnimateClosing, onClosed);
        }

        #endregion

        #region Back

        public virtual bool Back(WindowTransition transition = WindowTransition.AnimateClosing, Action onClosed = null)
        {
            WindowClosing?.Invoke(_navStack.Peek().WindowController);

            var result = _navStack.Pop(transition, out var controller, onClosed);
            if (result)
            {
                WindowChanged?.Invoke(controller);
            }

            return result;
        }

        public bool Back(Action onClosed)
        {
            return Back(WindowTransition.AnimateClosing, onClosed);
        }

        public virtual bool Back(out IWindowController window,
            WindowTransition transition = WindowTransition.AnimateClosing,
            Action onClosed = null)
        {
            WindowClosing?.Invoke(_navStack.Peek().WindowController);

            var result = _navStack.Pop(transition, out window, onClosed);
            if (result)
            {
                WindowChanged?.Invoke(window);
            }

            return result;
        }

        public bool Back(out IWindowController window, Action onClosed)
        {
            return Back(out window, WindowTransition.AnimateClosing, onClosed);
        }

        #endregion

        private T OpenBase<T>() where T : IWindowController, new()
        {
            var controller = new T();
            var view = GetViewForController(controller);

            controller.Initialize(this, view);

            return controller;
        }

        private AWindowView GetViewForController<T>(T controller) where T : IWindowController, new()
        {
            return _root.GetViewOrSpawn(controller.ViewType,
                                        () => _settings.Views.First(v => v.GetType() == controller.ViewType));
        }

        protected void EscapeListenerOnEscape()
        {
            Escape?.Invoke();
        }

        protected void RootOnDestroying()
        {
            _escapeListener.Escape -= EscapeListenerOnEscape;

            while (_navStack.Count > 0)
            {
                Back(WindowTransition.None);
            }
        }
    }
}