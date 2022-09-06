using UnityEditor;
using UnityEngine;

namespace MemoryArt.Editor
{
    public static class Tools
    {
        [MenuItem("Tools/Clear All Data")]
        public static void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        [MenuItem("Tools/Reserialize All")]
        public static void ReserializeAll()
        {
            AssetDatabase.ForceReserializeAssets();
        }
    }
}