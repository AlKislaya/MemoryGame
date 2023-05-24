using System;
using Dainty.UI.WindowBase;

namespace Dainty.UI.Interfaces
{
    public interface IUiManager
    {
        UiRoot UiRoot { get; }
        IWindowController CurrentWindow { get; }
        int WindowsCount { get; }

        event Action Escape;
        event Action<IWindowController> WindowChanged;
        event Action<IWindowController> WindowClosing;

        bool IsOpened<T>();

    #region Open

        T Open<T>(bool isPopup = false, WindowTransition transition = WindowTransition.AnimateOpening)
            where T : IWindowController, new();
        T Open<T>(WindowTransition transition)
            where T : IWindowController, new();

        T Open<T, TS>(TS data, bool isPopup, WindowTransition transition = WindowTransition.AnimateOpening)
            where T : IWindowController, IConfigurableWindow<TS>, new();

        T Open<T, TS>(TS data, WindowTransition transition = WindowTransition.AnimateOpening)
            where T : IWindowController, IConfigurableWindow<TS>, new();

    #endregion

    #region Close

        bool Close<T>(WindowTransition transition = WindowTransition.AnimateClosing, Action onClosed = null);
        bool Close<T>(Action onClosed);

    #endregion

    #region Back

        bool Back(WindowTransition transition = WindowTransition.AnimateClosing, Action onClosed = null);
        bool Back(Action onClosed);

        bool Back(out IWindowController window, WindowTransition transition = WindowTransition.AnimateClosing,
            Action onClosed = null);

        bool Back(out IWindowController window, Action onClosed);

    #endregion
    }
}