using Dainty.UI.WindowBase;
using UnityEngine;

namespace Dainty.UI
{
    [CreateAssetMenu(menuName = "Dainty/UI/Settings", fileName = nameof(UiManagerSettings))]
    public class UiManagerSettings : ScriptableObject
    {
#pragma warning disable 649
        [SerializeField] private AWindowView[] _views;
#pragma warning restore 649

        public AWindowView[] Views => _views;
    }
}