using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResultsListManager : MonoBehaviour
{
    [SerializeField] private Transform listContainer; // Parent transform where items will be added
    [SerializeField] private GameObject simulacionCuadroPrefab; // The SimulacionCuadro prefab

    void Start()
    {
        DisplayAllResults();
    }

    private void DisplayAllResults()
    {
        List<SimResultData> results = SimulationDataManager.GetResults();
        
        if (results != null && results.Count > 0)
        {
            // Clear any existing items (except the template)
            ClearExistingItems();
            
            // Create an item for each result
            foreach (SimResultData result in results)
            {
                CreateResultItem(result);
            }
        }
        else
        {
            Debug. LogWarning("No simulation results found");
        }
    }

    private void CreateResultItem(SimResultData result)
    {
        // Instantiate a new SimulacionCuadro
        GameObject newItem = Instantiate(simulacionCuadroPrefab, listContainer);
        newItem.SetActive(true); // Make sure it's active
        
        // Find the Duration text in the new item
        TMP_Text durationText = newItem.transform.Find("Duration"). GetComponent<TMP_Text>();
        
        if (durationText != null)
        {
            string formattedTime = FormatTime(result.Seconds);
            durationText.text = formattedTime;
            Debug.Log("Created item with time: " + formattedTime);
        }
        else
        {
            Debug. LogWarning("Duration text not found in SimulacionCuadro prefab!");
        }
    }

    private void ClearExistingItems()
    {
        // Destroy all children except the template
        foreach (Transform child in listContainer)
        {
            if (child.gameObject != simulacionCuadroPrefab)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Call this when you want to refresh the list (e.g., after loading Results scene)
    public void RefreshList()
    {
        DisplayAllResults();
    }
}