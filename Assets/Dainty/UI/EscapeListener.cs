using System;
using UnityEngine;

namespace Dainty.UI
{
    public class EscapeListener : MonoBehaviour
    {
        public static EscapeListener Instance { get; private set; }

        public static event Action Escape;

        /// <summary>
        /// Returns false if already initialized, otherwise true
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            if (Instance != null)
            {
                return false;
            }

            var listener = new GameObject("EscapeListener");
            DontDestroyOnLoad(listener);
            Instance = listener.AddComponent<EscapeListener>();
            return true;
        }

#if UNITY_EDITOR || UNITY_ANDROID
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Escape?.Invoke();
            }
        }
#endif
    }
}