using System;
using Dainty.UI.WindowBase;

namespace Dainty.UI.Interfaces
{
    public interface INavigationStack
    {
        int Count { get; }

        void Push(IWindowController window, WindowTransition transition, bool isPopup = false);
        bool Pop(WindowTransition transition, out IWindowController window, Action onClosed);
        NavigationElement Peek();

        bool Close<T>(WindowTransition transition, Action onClosed);

        bool IsOpened<T>();
    }
}