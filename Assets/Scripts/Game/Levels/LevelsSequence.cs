using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSequence", menuName = "ScriptableObjects/LevelsSequence")]
public class LevelsSequence : ScriptableObject
{
    public List<Level> Levels;
}

[Serializable]
public class Level
{
    public Sprite Preview;
    public List<LevelObject> LevelObjects;
}

[Serializable]
public class LevelObject
{
    public TextAsset SvgTextAsset;
    public List<LevelObjectSettings> CopiesSettings;
}

[Serializable]
public class LevelObjectSettings
{
    public List<LevelObjectColors> PaintableColors;

    [Header("Transform")]
    public Vector2 Position;
    public Vector2 Scale = Vector2.one;
    public float Rotation = 0;
}

[Serializable]
public class LevelObjectColors
{
    public string Key;
    public Color Color;
}