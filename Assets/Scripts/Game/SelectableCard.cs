using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCard : PlayableCard
{
    public event Action<int> OnButtonClicked;
    [HideInInspector] public int Index;
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(() => OnButtonClicked?.Invoke(Index));
    }
}
