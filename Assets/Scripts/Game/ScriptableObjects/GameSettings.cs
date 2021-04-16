using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Settings/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{

    [Header("Zoom Settings")]
    public float MinZoomValue = 1f;
    public float MaxZoomValue = 3f;

    [Header("Timer Settings")]
    public int TimerSeconds = 5;
    public Color TimerColor;

    [Header("Progress Counter")]
    public Color ProgressColor;

    [Header("Done Counter")]
    public Color DoneColor;
}