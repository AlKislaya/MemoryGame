using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelObject
{
    public TextAsset SvgTextAsset;
    public bool IsStatic = false;
    public LevelObjectSettings QuestionSignSettings;
    public List<LevelObjectColors> ColorsSettings;
    public List<LevelObjectSettings> CopiesSettings;
}
