using UnityEngine;
using UnityEngine.UI;

namespace MemoryArt.Global
{
    public class StarController : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private Image _innerShadowImage;
        [SerializeField] private Image _flareImage;

        [Header("Active Star")] [SerializeField]
        private Color32 _fillActive;

        [SerializeField] private Color32 _innerShadowActive;
        [SerializeField] private Color32 _flareActive;

        [Header("Unactive Star")] [SerializeField]
        private Color32 _fillUnactive;

        [SerializeField] private Color32 _innerShadowUnactive;
        [SerializeField] private Color32 _flareUnactive;

        private bool _isActive = true;

        public bool IsActive => _isActive;

        public void SetActiveState(bool isActive)
        {
            if (isActive == _isActive)
            {
                return;
            }

            _isActive = isActive;

            if (isActive)
            {
                _fillImage.color = _fillActive;
                _innerShadowImage.color = _innerShadowActive;
                _flareImage.color = _flareActive;
            }
            else
            {
                _fillImage.color = _fillUnactive;
                _innerShadowImage.color = _innerShadowUnactive;
                _flareImage.color = _flareUnactive;
            }
        }
    }
}