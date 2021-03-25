using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSequence", menuName = "ScriptableObjects/LevelsSequence")]
public class LevelsTextAssetsSequence : ScriptableObject
{
    public List<TextAsset> SvgLevelsAssets;
}