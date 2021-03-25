using System;
using System.Collections.Generic;

[Serializable]
public class LevelsSequence
{
    public List<Level> Levels;
}

[Serializable]
public class Level
{
    //public bool IsLocked;
    public bool IsPassed;
    public float PassedPercents;
}