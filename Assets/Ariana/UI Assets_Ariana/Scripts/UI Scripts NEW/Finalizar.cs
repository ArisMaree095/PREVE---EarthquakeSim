using UnityEngine;
using UnityEngine.UI;

public class Finalizar : MonoBehaviour
{
    public Button QuitApp;

    void Start()
    {
        QuitApp.onClick.AddListener(Application.Quit);
    }
}
