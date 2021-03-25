using System;
using Dainty.UI.Interfaces;
using UnityEngine;

namespace Dainty.UI.WindowBase
{
    public interface IWindowController : IDisposable
    {
        Transform ViewTransform { get; }
        Type ViewType { get; }
        string WindowId { get; }

        void Initialize(IUiManager uiManager, AWindowView view);
        void OnOpened();

        void Subscribe();
        void UnSubscribe();

        void BeforeShow();
        void Show(bool animation = true, Action animationFinished = null);
        void Close(bool animation = true, Action animationFinished = null);
    }
}