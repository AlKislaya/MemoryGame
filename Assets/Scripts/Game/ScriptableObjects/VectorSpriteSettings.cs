using UnityEngine;

[CreateAssetMenu(fileName = "VectorSpriteSettings", menuName = "ScriptableObjects/Settings/VectorSpriteSettings", order = 0)]
public class VectorSpriteSettings : ScriptableObject
{
    [Header("Import Settings")]
    public string PaintableGroupKey = "Paintable";
    public Rect SceneRect = new Rect(0, 0, 480, 320);
    public int PixelsPerUnit = 100;
    public int TargetResolution = 480;
    public ushort GradientResolution = 64;

    [Header("Colors Settings")]
    public float StrokeHalfThickness;
    public Color ClearFillColor;
    public Color HighlightedStrokeColor;
    public Color RightStrokeColor;
    public Color WrongStrokeColor;
}