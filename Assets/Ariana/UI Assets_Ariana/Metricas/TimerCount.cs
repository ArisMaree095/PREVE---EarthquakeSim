using UnityEngine;

public class TimerCount : MonoBehaviour
{
    public static TimerCount instance;

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    //When pause menu is called
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    //To resume timer
    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    // Get the current elapsed time
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    //When the simulation is completed, save the timer. 
    public void SaveTimer()
    {
        StopTimer();
        Debug.Log("Time elapsed: " + elapsedTime);

        SimulationDataManager.SaveNewResult(elapsedTime);
    }

    public void EndSimulation()
    {
        TimerCount.instance.SaveTimer();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    // Reset timer for new simulation
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = false;
    }
}
