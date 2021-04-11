using Dainty.UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    //private Button _button;
    private void Awake()
    {
        //_button = GetComponent<Button>();
        GetComponent<Button>().onClick.AddListener(OnBackButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        ApplicationController.Instance.UiManager.Back(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
    }
}
