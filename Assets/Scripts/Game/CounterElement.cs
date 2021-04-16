﻿using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CounterElement : MonoBehaviour
{
    public event Action OnButtonClicked; 
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _filledOutline;
    [SerializeField] private Button _button;
    [SerializeField] private Image _tickImage;

    private Tween _animation;
    private Sequence _transformToButtonAnimation;

    //add button click listener, init transform to btn animation
    private void Start()
    {
        _button.onClick.AddListener(onButtonClicked);
        _transformToButtonAnimation = DOTween.Sequence()
            .AppendCallback(() =>
            {
                _tickImage.enabled = true;
                _text.enabled = false;
            })
            .Append(transform.DOScale(1.2f, .3f))
            .Append(transform.DOScale(1f, .3f))
            .AppendCallback(() =>
            {
                _button.enabled = true;
            }).Pause().SetAutoKill(false);
    }

    private void onButtonClicked()
    {
        OnButtonClicked?.Invoke();
    }

    public void SetDefaults()
    {
        transform.localScale = Vector3.one;
        _button.enabled = false;
        _tickImage.enabled = false;
        _text.enabled = true;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public Sequence TimerTween(int seconds, string secondsText)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_filledOutline.DOFillAmount(0, seconds).SetEase(Ease.Linear));
        for (int i = 0; i <= seconds; i++)
        {
            var currSeconds = seconds - i;
            sequence.InsertCallback(i, () => SetText($"{currSeconds}{secondsText}"));
        }

        return sequence;
    }

    public void SetAmount(float fillAmount, float duration = 0, Ease ease = Ease.Linear)
    {
        _animation?.Kill();
        if (duration == 0)
        {
            _filledOutline.fillAmount = fillAmount;
        }
        else
        {
            _animation = _filledOutline.DOFillAmount(fillAmount, duration).SetEase(ease);
            _animation.Play();
        }
    }

    public void SetColor(Color color)
    {
        _filledOutline.color = color;
    }

    public void TransformToButton(string text, Color color)
    {
        if (_animation != null && _animation.IsPlaying())
        {
            _animation.OnComplete(() =>
            {
                SetColor(color);
                _transformToButtonAnimation.Restart();
            });
        }
        else
        {
            SetColor(color);
            _transformToButtonAnimation.Restart();
        }
    }
}