using System;
using System.Collections.Generic;

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