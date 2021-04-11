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
            var openingWindow = new NavigationElement(window, isPopup);
            _stack.Push(openingWindow);
            window.ViewTransform.SetAsLastSibling();
            DoPushTransition(transition, closingWindow, openingWindow, null);
        }

        public bool Pop(WindowTransition transition, out IWindowController window, Action onClosed)
        {
            if (_stack.Count < 1)
            {
                window = null;
                return false;
            }

            var closingWindow = _stack.Pop();
            var openingWindow = _stack.Count > 0 ? _stack.Peek().WindowController : null;

            DoPopTransition(transition, closingWindow, openingWindow, onClosed);

            window = openingWindow;
            return true;
        }

        public NavigationElement Peek()
        {
            return _stack.Peek();
        }

        public bool Close<T>(WindowTransition transition, out IWindowController window, Action onClosed)
        {
            if (_stack.Peek().WindowController.GetType() == typeof(T))
            {
                return Pop(transition, out window, onClosed);
            }

            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                if (_stack[i].WindowController.GetType() == typeof(T))
                {
                    var closingWindow = _stack.RemoveAt(i);
                    var openingWindow = i > 0 ? _stack[i - 1].WindowController : null;

                    DoPopTransition(WindowTransition.None, closingWindow, openingWindow, onClosed);
                    window = openingWindow;
                    return true;
                }
            }

            window = null;
            return false;
        }

        public bool IsOpened<T>()
        {
            for (var i = _stack.Count; i >= 0; i--)
            {
                if (_stack[i].WindowController.GetType() == typeof(T))
                {
                    return true;
                }

                if (!_stack[i].IsPopup)
                {
                    return false;
                }
            }

            return false;
        }

        private static void DoPushTransition(WindowTransition transition, IWindowController closingWindow,
            NavigationElement openingWindow, Action animationFinished)
        {
            var openingWindowController = openingWindow.WindowController;
            closingWindow?.UnSubscribe();

            if (transition == WindowTransition.None)
            {
                if (!openingWindow.IsPopup)
                {
                    closingWindow?.Close(false, false);
                }

                openingWindowController.BeforeShow();
                openingWindowController.Show(true, false, () =>
                {
                    openingWindowController.Subscribe();
                    animationFinished?.Invoke();
                });
            }
            else if (transition == WindowTransition.AnimateClosing)
            {
                if (openingWindow.IsPopup)
                {
                    openingWindowController.BeforeShow();
                    openingWindowController.Show(true, false);
                    openingWindowController.Subscribe();
                    animationFinished?.Invoke();

#if DEV_LOG
                    UnityEngine.Debug.LogWarning("This is a popup window, so the previous window will not be closed");
#endif
                }
                else
                {
                    openingWindowController.BeforeShow();
                    openingWindowController.Show(true, false);

                    if (closingWindow != null)
                    {
                        closingWindow.Close(false, true, () =>
                        {
                            openingWindowController.Subscribe();
                            animationFinished?.Invoke();
                        });
                    }
                    else
                    {
                        openingWindowController.Subscribe();
                        animationFinished?.Invoke();
                    }
                }
            }
            else if (transition == WindowTransition.AnimateOpening)
            {
                openingWindowController.BeforeShow();
                if (!openingWindow.IsPopup && closingWindow != null)
                {
                    openingWindowController.Show(true, true, () =>
                    {
                        closingWindow.Close(false, false);

                        openingWindowController.Subscribe();
                        animationFinished?.Invoke();
                    });
                }
                else
                {
                    openingWindowController.Show(true, true, () =>
                    {
                        openingWindowController.Subscribe();
                        animationFinished?.Invoke();
                    });
                }
            }
            else if (transition == (WindowTransition.AnimateOpening | WindowTransition.AnimateClosing))
            {
                var animsFinished = 0;

                openingWindowController.BeforeShow();
                if (!openingWindow.IsPopup && closingWindow != null)
                {
                    closingWindow.Close(false, true, () =>
                    {
                        animsFinished++;
                        if (animsFinished == 2)
                        {
                            openingWindowController.Subscribe();
                            animationFinished?.Invoke();
                        }
                    });

                    openingWindowController.Show(true, true, () =>
                    {
                        animsFinished++;
                        if (animsFinished == 2)
                        {
                            openingWindowController.Subscribe();
                            animationFinished?.Invoke();
                        }
                    });
                }
                else
                {
                    openingWindowController.Show(true, true, () =>
                    {
                        openingWindowController.Subscribe();
                        animationFinished?.Invoke();
                    });
                }
            }
        }

        private static void DoPopTransition(WindowTransition transition, NavigationElement closingWindow,
            IWindowController openingWindow, Action animationFinished)
        {
            var closingWindowController = closingWindow.WindowController;
            closingWindowController.UnSubscribe();

            if (transition == WindowTransition.None)
            {
                if (openingWindow != null)
                {
                    closingWindowController.Close(true, false);
                    closingWindowController.Dispose();

                    openingWindow.BeforeShow();
                    if (!closingWindow.IsPopup)
                    {
                        openingWindow.Show(false, false, () =>
                        {
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
                else
                {
                    closingWindowController.Close(true, false);
                    closingWindowController.Dispose();
                    animationFinished?.Invoke();
                }
            }
            else if (transition == WindowTransition.AnimateClosing)
            {
                closingWindowController.Close(true, true, () =>
                {
                    closingWindowController.Dispose();
                    if (openingWindow != null)
                    {
                        openingWindow.BeforeShow();
                        if (!closingWindow.IsPopup)
                        {
                            openingWindow.Show(false, false, () =>
                            {
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
                    else
                    {
                        animationFinished?.Invoke();
                    }
                });
            }
            else if (transition == WindowTransition.AnimateOpening)
            {
                if (openingWindow != null)
                {
                    openingWindow.BeforeShow();
                    if (closingWindow.IsPopup)
                    {
                        closingWindowController.Close(true, false);
                        closingWindowController.Dispose();

                        openingWindow.Subscribe();
                        animationFinished?.Invoke();
                    }
                    else
                    {
                        openingWindow.Show(false, true, () =>
                        {
                            closingWindowController.Close(true, false);
                            closingWindowController.Dispose();

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
                    closingWindowController.Close(true, false);
                    closingWindowController.Dispose();
                    animationFinished?.Invoke();
                }
            }
            else if (transition == (WindowTransition.AnimateOpening | WindowTransition.AnimateClosing))
            {
                if (openingWindow != null)
                {
                    openingWindow.BeforeShow();
                    if (closingWindow.IsPopup)
                    {
                        closingWindowController.Close(true, true, () =>
                        {
                            closingWindowController.Dispose();
                            openingWindow.Subscribe();
                            animationFinished?.Invoke();
                        });
                    }
                    else
                    {
                        var animsFinished = 0;

                        closingWindowController.Close(true, true, () =>
                        {
                            closingWindowController.Dispose();
                            animsFinished++;
                            if (animsFinished == 2)
                            {
                                openingWindow.Subscribe();
                                animationFinished?.Invoke();
                            }
                        });

                        openingWindow.Show(false, true, () =>
                        {
                            animsFinished++;
                            if (animsFinished == 2)
                            {
                                openingWindow.Subscribe();
                                animationFinished?.Invoke();
                            }
                        });
                    }
                }
                else
                {
#if DEV_LOG
                    UnityEngine.Debug.LogWarning("There is no window that can be opened with animation!");
#endif
                    closingWindowController.Close(true, true, () =>
                    {
                        closingWindowController.Dispose();
                        animationFinished?.Invoke();
                    });
                }
            }
        }
    }
}