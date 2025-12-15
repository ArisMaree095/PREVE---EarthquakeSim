using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby_UI : MonoBehaviour
{
    public GameObject resultsPanel;      // The Panel containing the texts
    public TextMeshProUGUI timerText;    // Text for "Time: 00:00"
    public TextMeshProUGUI statusText;   // Text for "Passed/Failed"

    private void Start()
    {
        // 1. If no manager or no data (first launch), hide results
        if (DataManager.Instance == null || !DataManager.Instance.HasData)
        {
            if (resultsPanel != null) resultsPanel.SetActive(false);
            return;
        }

        // 2. We have data, so show the panel
        if (resultsPanel != null) resultsPanel.SetActive(true);

        // 3. Format and Display Time
        float t = DataManager.Instance.TotalTimeSpent;
        string minutes = Mathf.FloorToInt(t / 60).ToString("00");
        string seconds = Mathf.FloorToInt(t % 60).ToString("00");

        if (timerText != null)
            timerText.text = $"Time: {minutes}:{seconds}";

        // 4. Format and Display Status
        bool success = DataManager.Instance.DidDropCorrectly;

        if (statusText != null)
        {
            if (success)
                statusText.text = "Reaction: <color=green>SAFE (Dropped)</color>";
            else
                statusText.text = "Reaction: <color=red>UNSAFE (Stood Up)</color>";
        }
    }
}