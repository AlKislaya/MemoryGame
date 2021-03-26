using DG.Tweening;
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
    [SerializeField] private Transform _shadow;
    private Tween _shadowAnimation;

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
        _shadowAnimation?.Kill();
        _shadowAnimation = _shadow.DOScale(state ? Vector3.one : Vector3.zero, .5f);
        //LayoutRebuilder.MarkLayoutForRebuild(transform.parent.GetComponent<RectTransform>());
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
