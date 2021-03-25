using System;
using Dainty.UI.WindowBase;
using UnityEngine;

public class GameController : AWindowController<GameView>
{
    public override string WindowId { get; }

    private TextAsset _svgLevelAsset;

    public override void BeforeShow()
    {
        _svgLevelAsset = LevelsManager.Instance.CurrentLevelSvgTextAsset;
        view.InitGame(_svgLevelAsset);
    }
}