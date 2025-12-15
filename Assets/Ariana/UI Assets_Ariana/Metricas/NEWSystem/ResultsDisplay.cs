using TMPro;
using UnityEngine;

public class ResultsDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI statusText;
    public GameObject resultsPanel; // Optional: Hide panel if no data exists

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Check if we have the manager and if there is data to show
        if (DataManager.Instance != null && DataManager.Instance.HasData)
        {
            if (resultsPanel != null) resultsPanel.SetActive(true);

            // 1. Format Time (Minutes:Seconds)
            float time = DataManager.Instance.TotalTimeSpent;
            string minutes = Mathf.FloorToInt(time / 60).ToString("00");
            string seconds = Mathf.FloorToInt(time % 60).ToString("00");
            timeText.text = $"Time in Sim: {minutes}:{seconds}";

            // 2. Format Drop Status
            bool didDrop = DataManager.Instance.DidDropCorrectly;
            if (didDrop)
            {
                statusText.text = "Response: <color=green>SUCCESS (Dropped)</color>";
            }
            else
            {
                statusText.text = "Response: <color=red>FAILED (Did not Drop)</color>";
            }
        }
        else
        {
            // First time launching the game, hide the results or show "No Data"
            if (resultsPanel != null) resultsPanel.SetActive(false);
            timeText.text = "--:--";
            statusText.text = "Ready to start";
        }
    }
}