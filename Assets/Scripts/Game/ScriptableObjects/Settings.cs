using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance
    {
        get
        {
            return _instance;
        }
    }
    public VectorSpriteSettings VectorSpriteSettings => _vectorSpriteSettings;
    public GameSettings GameSettings => _gameSettings;

    [SerializeField] private VectorSpriteSettings _vectorSpriteSettings;
    [SerializeField] private GameSettings _gameSettings;

    private static Settings _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
