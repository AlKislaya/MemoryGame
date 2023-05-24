using System;
using UnityEngine;

namespace Dainty.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private bool _conformX = true;
        [SerializeField] private bool _conformY = true;
        [SerializeField] private bool _logging;
#pragma warning restore 649

        private RectTransform _panel;
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private Vector2Int _lastScreenSize = new Vector2Int(0, 0);
        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

        public event Action Changed;

        private void Awake()
        {
            _panel = GetComponent<RectTransform>();

            if (_panel == null)
            {
                Debug.LogError("Cannot apply safe area - no RectTransform found on " + name);
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            var safeArea = Screen.safeArea;

            if (safeArea != _lastSafeArea
                || Screen.width != _lastScreenSize.x
                || Screen.height != _lastScreenSize.y
                || Screen.orientation != _lastOrientation)
            {
                // Fix for having auto-rotate off and manually forcing a screen orientation.
                // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
                _lastScreenSize.x = Screen.width;
                _lastScreenSize.y = Screen.height;
                _lastOrientation = Screen.orientation;

                ApplySafeArea(safeArea);
            }
        }

        private void ApplySafeArea(Rect safeArea)
        {
            _lastSafeArea = safeArea;

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            // Ignore x-axis?
            if (!_conformX)
            {
                safeArea.x = 0;
                safeArea.width = screenWidth;
            }

            // Ignore y-axis?
            if (!_conformY)
            {
                safeArea.y = 0;
                safeArea.height = screenHeight;
            }

            // Check for invalid screen startup state on some Samsung devices (see below)
            if (screenWidth > 0 && screenHeight > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                var anchorMin = safeArea.position;
                var anchorMax = safeArea.position + safeArea.size;
                anchorMin.x /= screenWidth;
                anchorMin.y /= screenHeight;
                anchorMax.x /= screenWidth;
                anchorMax.y /= screenHeight;

                // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
                // See https://forum.unity.com/threads/569236/page-2#post-6199352
                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    _panel.anchorMin = anchorMin;
                    _panel.anchorMax = anchorMax;
                }
            }

            if (_logging)
            {
                Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                                name, safeArea.x, safeArea.y, safeArea.width, safeArea.height, screenWidth, screenHeight);
            }

            Changed?.Invoke();
        }
    }
}