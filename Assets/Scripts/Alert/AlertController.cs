using Dainty.UI.WindowBase;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AlertButtonSettings
{
    public string Text;
    public Color32 Color;
    public UnityAction Callback;
}
public class AlertSettings
{
    public string HeaderText;
    public string DialogText;
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