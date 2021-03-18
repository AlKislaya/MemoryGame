using UnityEngine;
using UnityEngine.EventSystems;

public class Zoom : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler
{
    private const float TouchZoomSpeed = 0.005f;
    [SerializeField] private Transform _zoomContainer;
    [SerializeField] private Camera _cameraMain;
    private float _zoomMinValue = 1f;
    private float _zoomMaxValue = 3f;

    private float _halfWidth;
    private float _halfHeight;
    private float _boundX;
    private float _boundY;

    private Vector3 _lastDragPos;

    void Start()
    {
        var _vectorSpriteSettings = Settings.Instance.VectorSpriteSettings;

        _halfWidth = _vectorSpriteSettings.SceneRect.width / _vectorSpriteSettings.PixelsPerUnit / 2;
        _halfHeight = _vectorSpriteSettings.SceneRect.height / _vectorSpriteSettings.PixelsPerUnit / 2;

        _boundX = _halfWidth * _zoomContainer.localScale.x - _halfWidth;
        _boundY = _halfHeight * _zoomContainer.localScale.y - _halfHeight;

        _zoomMinValue = Settings.Instance.GameSettings.MinZoomValue;
        _zoomMaxValue = Settings.Instance.GameSettings.MaxZoomValue;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var worldPosition = _cameraMain.ScreenToWorldPoint(eventData.position);
#if !UNITY_EDITOR
        if (Input.touchCount == 1)
        {
#endif
        var newPos = _zoomContainer.localPosition - (_lastDragPos - worldPosition);
        _lastDragPos = worldPosition;
        newPos.z = _zoomContainer.localPosition.z;
        ClampContainerInRect(newPos);
#if !UNITY_EDITOR
        }
        else
        if (Input.touchCount == 2)
        {
            // get current touch positions
            Touch tZero = Input.GetTouch(0);
            Touch tOne = Input.GetTouch(1);
            // get touch position from the previous frame
            Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
            Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

            float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
            float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

            // get offset value
            float deltaDistance = oldTouchDistance - currentTouchDistance;
            ChangeZoom(deltaDistance, TouchZoomSpeed);
        }
#endif
    }

    private void ChangeZoom(float deltaMagnitudeDiff, float speed)
    {
        _zoomContainer.localScale = new Vector3(
            Mathf.Clamp(_zoomContainer.localScale.x - deltaMagnitudeDiff * speed, _zoomMinValue, _zoomMaxValue),
            Mathf.Clamp(_zoomContainer.localScale.y - deltaMagnitudeDiff * speed, _zoomMinValue, _zoomMaxValue),
            0);

        _boundX = _halfWidth * _zoomContainer.localScale.x - _halfWidth;
        _boundY = _halfHeight * _zoomContainer.localScale.y - _halfHeight;

        ClampContainerInRect(_zoomContainer.localPosition);
    }

    private void ClampContainerInRect(Vector3 position)
    {
#if UNITY_EDITOR

        _boundX = _halfWidth * _zoomContainer.localScale.x - _halfWidth;
        _boundY = _halfHeight * _zoomContainer.localScale.y - _halfHeight;
#endif
        _zoomContainer.localPosition = new Vector3(
            Mathf.Clamp(position.x, -_boundX, _boundX),
            Mathf.Clamp(position.y, -_boundY, _boundY),
            position.z);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastDragPos = eventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _lastDragPos = eventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}