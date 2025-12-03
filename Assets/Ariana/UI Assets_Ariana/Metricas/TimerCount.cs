using UnityEngine;

public class TimerCount : MonoBehaviour
{
   public static TimerCount instance;

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
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

    //When the simulation is completed, save the timer.
    public void SaveTimer()
    {
        StopTimer();
        Debug.Log("Time elapsed: " + elapsedTime);

        SimulationDataManager.SaveNewResult(elapsedTime);
    }
}
