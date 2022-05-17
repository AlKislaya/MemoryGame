using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSequence", menuName = "ScriptableObjects/LevelsSequence")]
public class LevelsSequence : ScriptableObject
{
    public List<Level> Levels;
}