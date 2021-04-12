using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelObject
{
    public TextAsset SvgTextAsset;
    public bool IsStatic = false;
    public List<LevelObjectSettings> CopiesSettings;
}
