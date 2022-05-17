using System;
using System.Collections.Generic;
using Dainty.UI.WindowBase;
using UnityEngine.Events;

namespace MemoryArt.UI.Windows
{
    [Serializable]
    public enum AlertButtonColor
    {
        White,
        Green
    }

    public class AlertButtonSettings
    {
        public string Text;
        public AlertButtonColor Color;
        public UnityAction Callback;
    }

    public class AlertWindowSettings
    {
        public string HeaderText;
        public string DialogText;
        public UnityAction OnBackButtonClicked;
        public List<AlertButtonSettings> Buttons;
    }

    public class AlertWindowController : AWindowController<AlertWindowView>, IConfigurableWindow<AlertWindowSettings>
    {
        public override string WindowId { get; }

        public void Initialize(AlertWindowSettings data)
        {
            view.InitializeAlert(data);
        }
    }
}