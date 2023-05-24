using System;
using Dainty.UI.Interfaces;
using Dainty.UI.WindowBase;

namespace Dainty.UI
{
    public class NavigationStack : INavigationStack
    {
        private readonly ListStack<NavigationElement> _stack;

        public NavigationStack()
        {
            _stack = new ListStack<NavigationElement>();
        }

        public int Count => _stack.Count;

        public void Push(IWindowController window, WindowTransition transition, bool isPopup = false)
        {
            var closingWindow = _stack.Count > 0 ? _stack.Peek().WindowController : null;
            var openingWindowElement = new NavigationElement(window, isPopup);
            _stack.Push(openingWindowElement);
            window.ViewTransform.SetAsLastSibling();
            DoPushTransition(transition, closingWindow, openingWindowElement, null);
        }

        public bool Pop(WindowTransition transition, out IWindowController window, Action onClosed)
        {
            if (_stack.Count < 1)
            {
                window = null;
                return false;
            }

            var closingWindowElement = _stack.Pop();
            var openingWindowElement = _stack.Count > 0 ? _stack.Peek() : null;

            DoPopTransition(transition, closingWindowElement, openingWindowElement, onClosed);

            window = openingWindowElement?.WindowController;
            return true;
        }

        public NavigationElement Peek()
        {
            return _stack.Peek();
        }

        public bool Close<T>(WindowTransition transition, Action onClosed)
        {
            if (_stack.Peek().WindowController.GetType() == typeof(T))
            {
                return Pop(transition, out _, onClosed);
            }

            for (var i = _stack.Count - 2; i >= 0; i--)
            {
                if (_stack[i].WindowController.GetType() == typeof(T))
                {
                    var closingWindow = _stack.RemoveAt(i).WindowController;
                    closingWindow.UnSubscribe();
                    closingWindow.Close(true, false);
                    closingWindow.Dispose();

                    onClosed?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public bool IsOpened<T>()
        {
            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                if (_stack[i].WindowController.GetType() == typeof(T))
                {
                    return true;
                }
            }

            return false;
        }

        private void DoPushTransition(WindowTransition transition, IWindowController closingWindow,
            NavigationElement openingWindowElement, Action animationFinished)
        {
            var openingWindow = openingWindowElement.WindowController;
            closingWindow?.UnSubscribe();

            if (transition == WindowTransition.None)
            {
                if (!openingWindowElement.IsPopup)
                {
                    closingWindow?.Close(false, false);
                }

                openingWindow.BeforeShow();
                openingWindow.Show(true, false);
                openingWindow.Subscribe();
                animationFinished?.Invoke();
            }
            else if (transition == WindowTransition.AnimateClosing)
            {
                openingWindow.BeforeShow();
                openingWindow.Show(true, false);

                if (openingWindowElement.IsPopup)
                {
                    openingWindow.Subscribe();
                    animationFinished?.Invoke();

#if DEV_LOG
                    UnityEngine.Debug.LogWarning("This is a popup window, so the previous window will not be closed");
#endif
                }
                else
                {
                    if (closingWindow != null)
                    {
                        closingWindow.Close(false, true, () =>
                        {
                            if (Peek() == openingWindowElement)
                                openingWindow.Subscribe();
                            animationFinished?.Invoke();
                        });
                    }
                    else
                    {
                        openingWindow.Subscribe();
                        animationFinished?.Invoke();
                    }
                }
            }
            else if (transition == WindowTransition.AnimateOpening)
            {
                openingWindow.BeforeShow();
                if (!openingWindowElement.IsPopup && closingWindow != null)
                {
                    openingWindow.Show(true, true, () =>
                    {
                        closingWindow.Close(false, false);

                        if (Peek() == openingWindowElement)
                            openingWindow.Subscribe();
                        animationFinished?.Invoke();
                    });
                }
                else
                {
                    openingWindow.Show(true, true, () =>
                    {
                        if (Peek() == openingWindowElement)
                            openingWindow.Subscribe();
                        animationFinished?.Invoke();
                    });
                }
            }
            else if (transition == (WindowTransition.AnimateOpening | WindowTransition.AnimateClosing))
            {
                var animsFinished = 0;

                openingWindow.BeforeShow();
                if (!openingWindowElement.IsPopup && closingWindow != null)
                {
                    void OnAnimationFinished()
                    {
                        animsFinished++;
                        if (animsFinished == 2)
                        {
                            if (Peek() == openingWindowElement)
                                openingWindow.Subscribe();
                            animationFinished?.Invoke();
                        }
                    }

                    closingWindow.Close(false, true, OnAnimationFinished);
                    openingWindow.Show(true, true, OnAnimationFinished);
                }
                else
                {
                    openingWindow.Show(true, true, () =>
                    {
                        if (Peek() == openingWindowElement)
                            openingWindow.Subscribe();
                        animationFinished?.Invoke();
                    });
                }
            }
        }

        private void DoPopTransition(WindowTransition transition, NavigationElement closingWindowElement,
            NavigationElement openingWindowElement, Action animationFinished)
        {
            var closingWindow = closingWindowElement.WindowController;
            closingWindow.UnSubscribe();

            var openingWindow = openingWindowElement?.WindowController;

            if (transition == WindowTransition.None)
            {
                if (openingWindow != null)
                {
                    closingWindow.Close(true, false);
                    closingWindow.Dispose();

                    openingWindow.BeforeShow();
                    if (!closingWindowElement.IsPopup)
                    {
                        openingWindow.Show(false, false);
                    }

                    openingWindow.Subscribe();
                    animationFinished?.Invoke();
                }
                else
                {
                    closingWindow.Close(true, false);
                    closingWindow.Dispose();
                    animationFinished?.Invoke();
                }
            }
            else if (transition == WindowTransition.AnimateClosing)
            {
                if (openingWindow != null)
                {
                    openingWindow.BeforeShow();
                    openingWindow.Show(false, false);
                }

                closingWindow.Close(true, true, () =>
                {
                    closingWindow.Dispose();
                    if (openingWindowElement != null && Peek() == openingWindowElement)
                        openingWindow?.Subscribe();
                    animationFinished?.Invoke();
                });
            }
            else if (transition == WindowTransition.AnimateOpening)
            {
                if (openingWindow != null)
                {
                    openingWindow.BeforeShow();
                    if (closingWindowElement.IsPopup)
                    {
                        closingWindow.Close(true, false);
                        closingWindow.Dispose();

                        openingWindow.Subscribe();
                        animationFinished?.Invoke();
                    }
                    else
                    {
                        openingWindow.Show(false, true, () =>
                        {
                            closingWindow.Close(true, false);
                            closingWindow.Dispose();

                            if (Peek() == openingWindowElement)
                                openingWindow.Subscribe();
                            animationFinished?.Invoke();
                        });
                    }
                }
                else
                {
#if DEV_LOG
                    UnityEngine.Debug.LogWarning("There is no window that can be opened with animation!");
#endif
                    closingWindow.Close(true, false);
                    closingWindow.Dispose();
                    animationFinished?.Invoke();
                }
            }
            else if (transition == (WindowTransition.AnimateOpening | WindowTransition.AnimateClosing))
            {
                if (openingWindow != null)
                {
                    openingWindow.BeforeShow();
                    if (closingWindowElement.IsPopup)
                    {
                        closingWindow.Close(true, true, () =>
                        {
                            closingWindow.Dispose();
                            if (Peek() == openingWindowElement)
                                openingWindow.Subscribe();
                            animationFinished?.Invoke();
                        });
                    }
                    else
                    {
                        var animsFinished = 0;

                        void OnAnimationFinished()
                        {
                            animsFinished++;
                            if (animsFinished == 2)
                            {
                                if (Peek() == openingWindowElement)
                                    openingWindow.Subscribe();
                                animationFinished?.Invoke();
                            }
                        }

                        closingWindow.Close(true, true, () =>
                        {
                            closingWindow.Dispose();
                            OnAnimationFinished();
                        });
                        openingWindow.Show(false, true, OnAnimationFinished);
                    }
                }
                else
                {
#if DEV_LOG
                    UnityEngine.Debug.LogWarning("There is no window that can be opened with animation!");
#endif
                    closingWindow.Close(true, true, () =>
                    {
                        closingWindow.Dispose();
                        animationFinished?.Invoke();
                    });
                }
            }
        }
    }
}