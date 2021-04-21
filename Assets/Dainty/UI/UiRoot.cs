using System;
using Dainty.UI.WindowBase;
using UnityEngine;

namespace Dainty.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UiRoot : MonoBehaviour
    {
        private RectTransform _canvasRect;

        public Vector2 CanvasSize => _canvasRect.rect.size;

        public event Action Destroying;

        private void Awake()
        {
            _canvasRect = GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            Destroying?.Invoke();
        }

        public AWindowView GetViewOrSpawn(Type viewType, Func<AWindowView> prefab)
        {
            var view = (AWindowView) GetComponentInChildren(viewType);
            if (view == null)
            {
                view = Instantiate(prefab(), transform);
            }

            view.Initialize(this);
            return view;
        }

        public Rect GetSafeArea()
        {
            var safeArea = Screen.safeArea;

            var width = Screen.width;
            var height = Screen.height;
            var canvasSize = _canvasRect.rect.size;

            safeArea.x = safeArea.x * canvasSize.x / width;
            safeArea.y = safeArea.y * canvasSize.y / height;

            safeArea.width = safeArea.width * canvasSize.x / width;
            safeArea.height = safeArea.height * canvasSize.y / height;

            return safeArea;
        }
    }
}