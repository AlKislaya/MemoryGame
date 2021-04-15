using System;
using Dainty.UI.WindowBase;
using UnityEngine;

namespace Dainty.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UiRoot : MonoBehaviour
    {
        private Canvas _canvas;

        public Vector2 CanvasSize => _canvas.pixelRect.size;

        public event Action Destroying;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void OnDestroy()
        {
            Destroying?.Invoke();
        }

        public AWindowView GetViewOrSpawn(Type viewType, Func<AWindowView> prefab)
        {
            var view = (AWindowView)GetComponentInChildren(viewType);
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
            var canvasSize = _canvas.pixelRect.size;

            safeArea.x = safeArea.x * canvasSize.x / width;
            safeArea.y = safeArea.y * canvasSize.y / height;

            safeArea.width = safeArea.width * canvasSize.x / width;
            safeArea.height = safeArea.height * canvasSize.y / height;

            return safeArea;
        }
    }
}