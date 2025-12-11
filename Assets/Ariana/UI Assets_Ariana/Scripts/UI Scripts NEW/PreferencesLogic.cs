using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreferencesLogic : MonoBehaviour
{
    public Slider VolumeSlider;
    public Toggle TremorToggle;
    public Button SaveButton; // The "Aceptar" button

    void Start()
    {
        if (SaveButton) SaveButton.onClick.AddListener(SaveSettings);
    }

    public void UpdateVolume(float value)
    {
        AudioListener.volume = value;
    }

    void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", VolumeSlider.value);
        PlayerPrefs.SetInt("Tremor", TremorToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Settings Saved!");
    }
}
