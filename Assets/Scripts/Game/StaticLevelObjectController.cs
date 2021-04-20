using UnityEngine;
//stores individual static level object
public class StaticLevelObjectController : MonoBehaviour
{
    private const float Pivot = 2.5f;

    [SerializeField] private SpriteRenderer _staticSpriteRendererPrefab;

    public void Init(SvgLoader.StaticVectorSprite staticVectorSprite)
    {
        var staticSpriteInstance = Instantiate(_staticSpriteRendererPrefab, transform);
        staticSpriteInstance.sprite = staticVectorSprite.Sprite;
    }

    public void InitSettings(LevelObjectSettings settings, float zPos)
    {
        transform.localPosition = new Vector3(settings.Position.x - Pivot, Pivot - settings.Position.y, zPos);
        transform.localScale = new Vector3(settings.Scale.x, settings.Scale.y, 1);

        if (settings.Rotation != 0)
        {
            var rotation = transform.localRotation.eulerAngles;
            rotation.z = settings.Rotation;
            transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}
