using UnityEngine;

public class Settings : Singleton<Settings>
{
    public VectorSpriteSettings VectorSpriteSettings => _vectorSpriteSettings;
    public GameSettings GameSettings => _gameSettings;

    [SerializeField] private VectorSpriteSettings _vectorSpriteSettings;
    [SerializeField] private GameSettings _gameSettings;
}