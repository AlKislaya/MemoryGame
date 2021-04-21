using UnityEngine;

public class Settings : Singleton<Settings>
{
    public GameSettings GameSettings => _gameSettings;

    [SerializeField] private GameSettings _gameSettings;
}