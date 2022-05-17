using System;
using System.Collections.Generic;

namespace MemoryArt.Game.Levels
{
    [Serializable]
    public class LevelsProgress
    {
        public List<LevelProgress> Levels;
    }

    [Serializable]
    public class LevelProgress
    {
        public bool IsPassed;
        public float PassedPercents;
    }
}