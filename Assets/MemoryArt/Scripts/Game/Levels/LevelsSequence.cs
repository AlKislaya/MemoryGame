using System.Collections.Generic;
using UnityEngine;

namespace MemoryArt.Game.Levels
{
    [CreateAssetMenu(fileName = "LevelsSequence", menuName = "ScriptableObjects/LevelsSequence")]
    public class LevelsSequence : ScriptableObject
    {
        public List<Level> Levels;
    }
}