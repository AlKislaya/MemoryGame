using System;
using UnityEngine;

namespace MemoryArt.Game.Levels
{
    [Serializable]
    public class LevelsCategory
    {
        public string Key;
        public int Price;
        public Sprite Preview;
        public LevelsSequence LevelsSequence;
    }
}