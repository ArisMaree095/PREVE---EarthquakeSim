using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationOver : MonoBehaviour
{
    public TMP_Text timeDisplayText;

    void Start()
    {
        DisplayLastSessionTime();
    }

    private void DisplayLastSessionTime()
    {
        SimResultData lastResult = SimulationDataManager.GetMostRecentResult();
        if (lastResult != null)
        {
            float timeInSeconds = lastResult.Seconds;
            string formattedTime = FormatTime(timeInSeconds);
        
        if (timeDisplayText != null)
        {
            timeDisplayText.text = "Duración: " + formattedTime;
        } 
        else
        {
            Debug.LogWarning("timeDisplayText reference is not set in the inspector.");
        } 
        }
        else
        {
            if(timeDisplayText != null) timeDisplayText.text = "No hay datos";

        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
