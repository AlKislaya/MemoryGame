using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class PlayableCard : MonoBehaviour
{
    [SerializeField] protected bool _isOpen;
    [SerializeField] protected Transform _cardRoot;
    [SerializeField] protected Transform _imagesContainer;
    [SerializeField] protected SVGImage _imagePrefab;

    protected List<Transform> _rootChildren;
    protected List<SVGImage> _imageInstances = new List<SVGImage>();

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
