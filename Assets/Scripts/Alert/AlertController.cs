using Dainty.UI.WindowBase;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

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
public class AlertSettings
{
    public string HeaderText;
    public string DialogText;
    public UnityAction OnBackButtonClicked;
    public List<AlertButtonSettings> Buttons;
}

public class AlertController : AWindowController<AlertView>, IConfigurableWindow<AlertSettings>
{
    public override string WindowId { get; }

    public void Initialize(AlertSettings data)
    {
        view.InitializeAlert(data);
    }
}