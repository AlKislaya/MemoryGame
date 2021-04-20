using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelObjectSettings
{
    [Header("Transform")]
    public Vector2 Position;
    public Vector2 Scale = Vector2.one;
    public float Rotation = 0;
}
