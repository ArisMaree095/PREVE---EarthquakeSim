using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby_UI : MonoBehaviour
{
    //Title Screen
    public GameObject PlayPanel;
    public Button AccederButton;

    //Selection Screen
    public GameObject SelectionPanel;
    public Button IniciarButton;
    public Button AjustesButton;
    public Button FinalizarButton;

    //Ajustes Screen
    public GameObject AjustesPanel;
    public Button MetricasButton;
    public Button SistemaButton;
    public Button AjustesRegresarButton;

    //Settings or Preferences
    public GameObject PreferencesPanel;
    public Slider VolumeSlider;
    public Slider BrightnessSlider;
    public Toggle TremorToggle;
    public Button PrefRegresarButton;

    private float currentVolume = 1.0f;
    private float currentBrightness = 1.0f;
    
    //Metrics List Screen
    public GameObject MetricsListPanel;
    public Button MetricsListRegresarButton;

    //Exit Screen and Confirmation Panel
    public GameObject ExitPanel;
    public Button ExitRegresarButton;
    public Button ExitAceptarButton;
   
    private void Start()
    {
        ShowPanel(PlayPanel);

        AccederButton.onClick.AddListener(() => ShowPanel(SelectionPanel));

        IniciarButton.onClick.AddListener(OnIniciarClicked);
        AjustesButton.onClick.AddListener(() => ShowPanel(AjustesPanel));
        FinalizarButton.onClick.AddListener(() => ShowPanel(ExitPanel));

        MetricasButton.onClick.AddListener(() => ShowPanel(MetricsListPanel));
        SistemaButton.onClick.AddListener(() => ShowPanel(PreferencesPanel));
        AjustesRegresarButton.onClick.AddListener(() => ShowPanel(PlayPanel));

        if (VolumeSlider) VolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (BrightnessSlider) BrightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        PrefRegresarButton.onClick.AddListener(() => ShowPanel(AjustesPanel));
        PrefRegresarButton.onClick.AddListener(OnPreferencesSaved);

        MetricsListRegresarButton.onClick.AddListener(() => ShowPanel(AjustesPanel));

        ExitRegresarButton.onClick.AddListener(() => ShowPanel(SelectionPanel));
        ExitAceptarButton.onClick.AddListener(OnFinalizarClicked);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        PlayPanel.SetActive(false);
        SelectionPanel.SetActive(false);
        AjustesPanel.SetActive(false);
        PreferencesPanel.SetActive(false);
        MetricsListPanel.SetActive(false);
        ExitPanel.SetActive(false);

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    public void OnIniciarClicked()
    {
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        currentVolume = value;
    }

    public void OnBrightnessChanged(float value)
    {
        currentBrightness = value;
        RenderSettings.ambientIntensity = value;
    }

    public void OnPreferencesSaved()
    {
        PlayerPrefs.SetFloat("Volume", currentVolume);
        PlayerPrefs.SetFloat("Brightness", currentBrightness);
        PlayerPrefs.SetInt("Tremor", TremorToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        ShowPanel(AjustesPanel);
    }

    public void OnFinalizarClicked()
    {
        Application.Quit();
    }
}