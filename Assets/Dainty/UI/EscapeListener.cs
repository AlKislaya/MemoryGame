using System;
using UnityEngine;

namespace Dainty.UI
{
    public class EscapeListener : MonoBehaviour
    {
        private static EscapeListener _instance;

        public static EscapeListener Instance
        {
            get
            {
                if (_instance == null)
                {
                    var listener = new GameObject("EscapeListener");
                    DontDestroyOnLoad(listener);
                    _instance = listener.AddComponent<EscapeListener>();
                }

                return _instance;
            }
        }

        public event Action Escape;

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