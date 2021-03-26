using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SwatchesController : MonoBehaviour
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
            if (_colorInstances.Count > i)
            {
                _colorInstances[i].UpdateColor(sortedColors[i]);
                _colorInstances[i].SetActive(true);
            }
            else
            {
                var newSwatch = Instantiate(_colorSwatchPrefab, _viewportContentTransform);
                _colorInstances.Add(newSwatch);
                newSwatch.Init(_toggleGroup, _swatchesImages[swatchImageNumber], sortedColors[i]);
            }

            swatchImageNumber++;
            if (swatchImageNumber == _swatchesImages.Count)
            {
                swatchImageNumber = 0;
            }
        }

        for (int i = sortedColors.Count; i < _colorInstances.Count; i++)
        {
            _colorInstances[i].SetActive(false);
        }
    }

    public bool TryGetSelectedColor(ref Color color)
    {
        if (!_toggleGroup.AnyTogglesOn())
        {
            return false;
        }

        var swatch = _colorInstances.FirstOrDefault(x => x.IsOn);
        if (swatch == null)
        {
            return false;
        }

        color = swatch.Color;
        return true;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    //reset toggles selection, close
    public void Close()
    {
        if (_toggleGroup.AnyTogglesOn())
        {
            _toggleGroup.SetAllTogglesOff();
        }

        gameObject.SetActive(false);
    }
}
