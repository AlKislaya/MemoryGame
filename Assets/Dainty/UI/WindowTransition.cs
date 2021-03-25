using System;

namespace Dainty.UI
{
    [Flags]
    public enum WindowTransition
    {
        None = 0,
        AnimateClosing = 1 << 0,
        AnimateOpening = 1 << 1
    }
}