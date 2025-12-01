using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby_UI : MonoBehaviour
{
    public GameObject PlayPanel;
    public Button AccederButton;

    public GameObject SelectionPanel;
    public Button IniciarButton;
    public Button AjustesButton;
    public Button FinalizarButton;

    public GameObject SettingsPanel;
    public Button MetricasButton;
    public Button SistemaButton;
    public Button SettingsRegresarButton;

    public GameObject PreferencesPanel;
    public Slider VolumeSlider;
    public Slider BrightnessSlider;
    public Toggle TremorToggle;
    public Button PrefRegresarButton;
    public Button PrefAceptarButton;

    public GameObject MetricsListPanel;
    public Button Sim1Button;
    public Button Sim2Button;
    public Button MetricsListRegresarButton;

    public GameObject MetricsDetailPanel;
    public Button MetricsDetailRegresarButton;

    public GameObject ExitPanel;
    public Button ExitRegresarButton;
    public Button ExitAceptarButton;

    private float currentVolume = 1.0f;
    private float currentBrightness = 1.0f;

    void Start()
    {
        ShowPanel(PlayPanel);

        AccederButton.onClick.AddListener(() => ShowPanel(SelectionPanel));

        IniciarButton.onClick.AddListener(OnIniciarClicked);
        AjustesButton.onClick.AddListener(() => ShowPanel(SettingsPanel));
        FinalizarButton.onClick.AddListener(() => ShowPanel(ExitPanel));

        MetricasButton.onClick.AddListener(() => ShowPanel(MetricsListPanel));
        SistemaButton.onClick.AddListener(() => ShowPanel(PreferencesPanel));
        SettingsRegresarButton.onClick.AddListener(() => ShowPanel(SelectionPanel));

        if (VolumeSlider) VolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (BrightnessSlider) BrightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        PrefRegresarButton.onClick.AddListener(() => ShowPanel(SettingsPanel));
        PrefAceptarButton.onClick.AddListener(OnPreferencesSaved);

        if (Sim1Button) Sim1Button.onClick.AddListener(OnSimulationSelected);
        if (Sim2Button) Sim2Button.onClick.AddListener(OnSimulationSelected);
        MetricsListRegresarButton.onClick.AddListener(() => ShowPanel(SettingsPanel));

        MetricsDetailRegresarButton.onClick.AddListener(() => ShowPanel(MetricsListPanel));

        ExitRegresarButton.onClick.AddListener(() => ShowPanel(SelectionPanel));
        ExitAceptarButton.onClick.AddListener(OnExitConfirmClicked);
    }

    private void ShowPanel(GameObject panelToShow)
    {
        PlayPanel.SetActive(false);
        SelectionPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        PreferencesPanel.SetActive(false);
        MetricsListPanel.SetActive(false);
        MetricsDetailPanel.SetActive(false);
        ExitPanel.SetActive(false);

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    void OnIniciarClicked()
    {
        SceneManager.LoadScene("SimulationScene");
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        currentVolume = value;
    }

    void OnBrightnessChanged(float value)
    {
        currentBrightness = value;
        RenderSettings.ambientIntensity = value;
    }

    void OnPreferencesSaved()
    {
        PlayerPrefs.SetFloat("Volume", currentVolume);
        PlayerPrefs.SetFloat("Brightness", currentBrightness);
        PlayerPrefs.SetInt("Tremor", TremorToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        ShowPanel(SettingsPanel);
    }

    void OnSimulationSelected()
    {
        ShowPanel(MetricsDetailPanel);
    }

    void OnExitConfirmClicked()
    {
        Application.Quit();
    }
}