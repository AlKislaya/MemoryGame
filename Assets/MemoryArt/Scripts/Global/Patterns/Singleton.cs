using UnityEngine;

namespace MemoryArt.Global.Patterns
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected static T _instance;

        public static T Instance => _instance;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this as T)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(_instance);
            }
        }
    }
}