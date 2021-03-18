using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ColorsSwatchesController : MonoBehaviour
{
    [SerializeField] private Transform _viewportContentTransform;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Swatch _colorSwatchPrefab;
    [SerializeField] private List<Sprite> _swatchesImages;
    private List<Swatch> _colorInstances = new List<Swatch>();

    public void AddColors(List<Color> colors)
    {
        var sortedColors = new List<Color>();
        foreach (var color in colors)
        {
            if (!sortedColors.Contains(color))
            {
                sortedColors.Add(color);
            }
        }

        var swatchImageNumber = 0;
        for (int i = 0; i < sortedColors.Count; i++)
        {
            var newSwatch = Instantiate(_colorSwatchPrefab, _viewportContentTransform);
            _colorInstances.Add(newSwatch);
            newSwatch.Init(_toggleGroup, _swatchesImages[swatchImageNumber], sortedColors[i]);

            swatchImageNumber++;
            if (swatchImageNumber == _swatchesImages.Count)
            {
                swatchImageNumber = 0;
            }
        }
    }

    public bool TryGetSelectedColor(ref Color color)
    {
        var swatch = _colorInstances.FirstOrDefault(x => x.IsOn);
        if (swatch == null)
        {
            return false;
        }
        else
        {
            color = swatch.Color;
            return true;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
