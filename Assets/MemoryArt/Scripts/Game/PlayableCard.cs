using System.Collections.Generic;
using DG.Tweening;
using Unity.VectorGraphics;
using UnityEngine;

namespace MemoryArt.Game
{
    public class PlayableCard : MonoBehaviour
    {
        private static float _halfScreenHeight;

        [SerializeField] protected bool _isOpen;
        [SerializeField] protected Transform _cardRoot;
        [SerializeField] protected Transform _imagesContainer;
        [SerializeField] protected SVGImage _imagePrefab;

        protected readonly List<SVGImage> _imageInstances = new List<SVGImage>();
        protected List<Transform> _rootChildren;
        protected RectTransform _rectTransform;

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _halfScreenHeight = ApplicationController.Instance.UiRoot.CanvasSize.y / 2;
        }

        public virtual void SetParent(RectTransform parent)
        {
            _rectTransform.SetParent(parent);
        }

        public virtual Tween DoMove(float duration)
        {
            return _rectTransform.DOLocalMove(Vector3.zero, duration);
        }

        public virtual void DoStretch()
        {
            _rectTransform.sizeDelta = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
            _rectTransform.offsetMin = Vector2.zero;
        }

        public virtual Tween DoStretch(float duration)
        {
            return _rectTransform.DOSizeDelta(Vector2.zero, duration);
        }

        public virtual Tween DoHideMove(float duration)
        {
            return _rectTransform
                .DOLocalMoveY(-_halfScreenHeight - (_rectTransform.rect.height / 2) - 20, duration)
                .SetEase(Ease.InBack);
        }

        public virtual void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public virtual void AddImage(Sprite sprite, int index)
        {
            if (_imageInstances.Count <= index)
            {
                for (int i = _imageInstances.Count; i <= index; i++)
                {
                    _imageInstances.Add(Instantiate(_imagePrefab, _imagesContainer));
                }
            }

            _imageInstances[index].sprite = sprite;
            _imageInstances[index].enabled = true;
        }

        public virtual void ResetCard()
        {
            foreach (var imageInstance in _imageInstances)
            {
                imageInstance.sprite = null;
                imageInstance.enabled = false;
            }
        }

        public virtual void OpenCard(bool isOpen)
        {
            if (isOpen == _isOpen)
            {
                return;
            }

            if (_rootChildren == null)
            {
                _rootChildren = new List<Transform>();
                for (int i = 0; i < _cardRoot.childCount; i++)
                {
                    _rootChildren.Add(_cardRoot.GetChild(i));
                }

                if (!_isOpen)
                {
                    _rootChildren.Reverse();
                }
            }

            for (int i = 0; i < _rootChildren.Count; i++)
            {
                _rootChildren[i].SetSiblingIndex(isOpen ? i : _rootChildren.Count - 1 - i);
            }

            _isOpen = isOpen;
        }
    }
}