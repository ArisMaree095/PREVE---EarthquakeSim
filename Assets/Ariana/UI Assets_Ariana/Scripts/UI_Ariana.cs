using UnityEngine;
using Meta;
using Meta.XR;
using UnityEngine.UI;

public class UI_Script : MonoBehaviour
{

    //Main Canvas
    public Canvas MainCanvas;

    public Button StartButton; //Start Simulations
    public Button SettingsButton; //Open Settings Menu
   // public Button Confirm; //OK Button
    public Button ExitButton; //Exit Application
    public Button BackButton; //Back from Settings

    //public GameObject ConfirmPanel; //Confirmation Panel

    public GameObject MainPanel; //Main Menu Panel
    public GameObject SettingsPanel; //Settings Menu Panel
    public GameObject MetricsPanel; //Simulation Metrics Panel

   // private GameObject currentActivePanel; //Track the currently active panel

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        MetricsPanel.SetActive(false);
       // ConfirmPanel.SetActive(false);

        //currentActivePanel = MainPanel;

        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        StartButton.onClick.AddListener(() =>
        {
            MainPanel.SetActive(false);
            //ConfirmPanel.SetActive(true);
           // currentActivePanel = ConfirmPanel;
        });

        SettingsButton.onClick.AddListener(() =>
        {
            MainPanel.SetActive(false);
            SettingsPanel.SetActive(true);
            //currentActivePanel = SettingsPanel;
        });

       /* Confirm.onClick.AddListener(() =>
        {
            HandleConfirmButtonPress();
        });

        ExitButton.onClick.AddListener(() =>
        {
            HandleExitButtonPress();
        });*/

        BackButton.onClick.AddListener(() =>
        {
            HandleBackButtonPress();
        });
    }

    /* private void HandleConfirmButtonPress()
    {
        if (currentActivePanel == ConfirmPanel)
        {
            ConfirmPanel.SetActive(false);
            MetricsPanel.SetActive(true);
            currentActivePanel = MetricsPanel;
            // Start simulation here
        }
        else if (currentActivePanel == SettingsPanel)
        {
            SettingsPanel.SetActive(false);
            MainPanel.SetActive(true);
            currentActivePanel = MainPanel;
        }
        else if (currentActivePanel == MetricsPanel)
        {
            MetricsPanel.SetActive(false);
            MainPanel.SetActive(true);
            currentActivePanel = MainPanel;
        }
    }*/

    private void HandleExitButtonPress()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void HandleBackButtonPress()
    {
        
    if (SettingsPanel.activeSelf)
        {
            SettingsPanel.SetActive(false);
            MainPanel.SetActive(true);
            //currentActivePanel = MainPanel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // XR Controller Input Handling
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            HandleXRConfirmInput();
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            HandleXRConfirmInput();
        }

        // Hand Gesture Detection (Optional - requires hand tracking)
        DetectHandGestures();
    }

    private void HandleXRConfirmInput()
    {
        // Check if a button is currently being looked at via raycasting
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            Button hitButton = hit.collider.GetComponent<Button>();
            if (hitButton != null)
            {
                hitButton.onClick.Invoke();
            }
        }
    }

    private void DetectHandGestures()
    {
        // Detect pinch gesture on either hand
        bool leftPinch = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger);
        bool rightPinch = OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);

        if (leftPinch || rightPinch)
        {
            HandleXRConfirmInput();
        }
    }
}
