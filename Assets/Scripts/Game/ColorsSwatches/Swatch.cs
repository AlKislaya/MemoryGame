using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Swatch : MonoBehaviour
{
    [HideInInspector] public Color Color;
    public bool IsOn = false;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _shadow;

    private void Start()
    {
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void Init(ToggleGroup toggleGroup, Sprite sprite, Color color)
    {
        _toggle.group = toggleGroup;
        _image.sprite = sprite;
        UpdateColor(color);
    }

    public void UpdateColor(Color color)
    {
        _image.color = color;
        Color = color;
    }

    private void OnToggleValueChanged(bool state)
    {
        IsOn = state;
        _shadow.gameObject.SetActive(IsOn);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
