using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShowPanel : MonoBehaviour
{
    public GameObject panelToOpen;  // Drag the next panel here
    public GameObject panelToClose; // Drag the current panel here (optional)

    private Button myButton;

    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(SwitchPanels);
    }

    void SwitchPanels()
    {
        if (panelToClose != null) panelToClose.SetActive(false);
        if (panelToOpen != null) panelToOpen.SetActive(true);
    }
}
