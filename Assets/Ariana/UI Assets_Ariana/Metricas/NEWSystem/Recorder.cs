using UnityEngine;
using UnityEngine.SceneManagement;

public class Recorder : MonoBehaviour
{
    public SistemaAgachadoVR sistemaAgachado; // DRAG YOUR CROUCH SCRIPT HERE
    public string lobbySceneName = "Lobby";

    // Data State
    private float _timer = 0f;
    private bool _earthquakeHasStarted = false;
    private bool _playerDroppedSuccess = false;
    private bool _isRecording = true;

    private void OnEnable()
    {
        // Listen for the earthquake signal from your existing manager
        TerremotoManager.Terremoto += OnEarthquakeStarted;
    }

    private void OnDisable()
    {
        TerremotoManager.Terremoto -= OnEarthquakeStarted;
    }

    private void Update()
    {
        if (_isRecording)
        {
            // 1. Count time
            _timer += Time.deltaTime;

            // 2. Check for Drop Response
            // We only check if the earthquake has started AND we haven't already succeeded
            if (_earthquakeHasStarted && !_playerDroppedSuccess)
            {
                if (sistemaAgachado != null && sistemaAgachado.EstaAgachado())
                {
                    _playerDroppedSuccess = true;
                    Debug.Log("✅ VALIDATION: Player is crouching during earthquake!");
                }
            }
        }
    }

    // Triggered automatically by your TerremotoTrigger
    private void OnEarthquakeStarted(AnimationCurve curva, float duracion)
    {
        if (!_earthquakeHasStarted)
        {
            _earthquakeHasStarted = true;
            Debug.Log("⚠ RECORDER: Earthquake started! Waiting for crouch...");
        }
    }

    // Call this when the level ends (e.g., exiting a door)
    public void FinishSimulation()
    {
        _isRecording = false;

        // Send data to the Persistent Singleton
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveData(_timer, _playerDroppedSuccess);
        }
        else
        {
            Debug.LogError("DataManager is missing! Start from the Lobby scene.");
        }

        // Return to Lobby
        SceneManager.LoadScene(lobbySceneName);
    }

    public void EndLevel()
    {
        // Put any logic here to stop data collection or save files
        Debug.Log("Recording stopped.");
    }
}
