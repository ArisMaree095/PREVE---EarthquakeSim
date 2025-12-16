using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public float TotalTimeSpent { get; private set; }
    public bool DidDropCorrectly { get; private set; }
    public bool HasData { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveData(float time, bool dropped)
    {
        TotalTimeSpent = time;
        DidDropCorrectly = dropped;
        HasData = true;
    }
}