using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public Sprite Preview;
    public List<LevelObject> LevelObjects;
}
