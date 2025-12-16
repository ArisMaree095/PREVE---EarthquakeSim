using UnityEngine;

public class EndSimulation : MonoBehaviour
{
    public Recorder recorder;             // Reference to data recorder
    public SimulationUI uiManager; 
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player hit the trigger
        if (other.CompareTag("Player"))
        {
            
            if (recorder != null)
            {
                recorder.EndLevel();
            }

            if (uiManager != null)
            {
                uiManager.ShowEndScreen();
            }
            else
            {
                Debug.LogError("UI Manager is not assigned in the Inspector!");
            }
        }
    }
}
