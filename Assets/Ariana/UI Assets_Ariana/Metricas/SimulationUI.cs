using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour
{
    public TMP_Text timerDisplayText; // Assign in Inspector

    private TimerCount timerCount;

    public Button Reiniciar;
    public Button Finalizar;
    public GameObject SimUI;

    public InputActionReference pauseInputAction;
    private bool isPaused = false;

    public void OnReiniciarButtonClicked()
    {
        SceneManager.LoadScene("PruebaDEJusset");
    }

    public void OnFinalizarButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPauseButtonClicked(bool pause)
    {
        if (pause)
        {
            SimUI.SetActive(true);
            timerCount?.StopTimer();
            isPaused = true;
        }
        else
        {
            SimUI.SetActive(false);
            timerCount?.ResumeTimer();
            isPaused = false;
        }
    }

    void Start()
    {
        SimUI.SetActive(false);

        timerCount = TimerCount.instance;
        if (timerCount == null)
        {
            Debug.LogError("TimerCount instance not found!");
        }
    }

    void Update()
    {
        if (timerCount != null && timerDisplayText != null)
        {
            float elapsedTime = timerCount.GetElapsedTime();
            string formattedTime = FormatTime(elapsedTime);
            timerDisplayText.text = formattedTime;
        }

        if (pauseInputAction != null && pauseInputAction.action.WasPerformedThisFrame())
        {
            OnPauseButtonClicked(!isPaused);
        }
    }
    void OnDisable()
    {
        if (pauseInputAction != null)
        {
            pauseInputAction.action.Disable();
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time % 1f) * 100f);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}
