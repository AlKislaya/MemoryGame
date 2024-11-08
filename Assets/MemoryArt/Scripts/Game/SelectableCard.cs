﻿using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryArt.Game
{
    public class SelectableCard : PlayableCard
    {
        [HideInInspector] public int Index;

        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _canvasGroup;

        private RectTransform _parentRect;

        public event Action<int> ButtonClick;

        protected override void Awake()
        {
            base.Awake();
            _parentRect = _rectTransform.parent.GetComponent<RectTransform>();
        }

        private void Start()
        {
            _button.onClick.AddListener(() => ButtonClick?.Invoke(Index));
        }

        public Tween DoFade(float alpha, float duration)
        {
            return _canvasGroup.DOFade(alpha, duration);
        }

        public void DoFade(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        public override void ResetCard()
        {
            base.ResetCard();
            if (_parentRect != null)
            {
                SetParent(_parentRect);
                DoStretch();
            }

            DoFade(1);
        }

        public override void SetActive(bool isActive)
        {
            _parentRect.gameObject.SetActive(isActive);
        }
    }
}