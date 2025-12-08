using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class SimResultData : MonoBehaviour
{
    public float Seconds;
    public long timestamp;

    public SimResultData(float time)
    {
        Seconds = time;
        timestamp = System.DateTime.Now.Ticks;
    }
}

[System.Serializable]
public class DataContainer //For easier JSON conversion
{
    public List <SimResultData> resultList = new List<SimResultData>();
}

public static class SimulationDataManager
{
    private static string saveFileName = "PREVE_Metrics.json";

    public static void SaveNewResult(float time)
    {
        DataContainer dataContainer = LoadDataContainer(); //Load the data first
        dataContainer.resultList.Add(new SimResultData(time)); //Add new data to list
        
       /* string json = JsonUtility.ToJson(dataContainer, true); //Convert to JSON
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), json);*/

        //Keep only the last 10 results
        if (dataContainer.resultList.Count > 10)
        {
            dataContainer.resultList.RemoveAt(0);
        }

        string Json = JsonUtility.ToJson(dataContainer, true); //Convert to JSON
        string path = Path.Combine(Application.persistentDataPath, saveFileName); 
    }

    public static List<SimResultData> GetResults()
    {
        return LoadDataContainer().resultList;
    }

    public static SimResultData GetMostRecentResult()
    {
        List<SimResultData> results = GetResults();
        if (results != null && results.Count > 0)
        {
            return results[results.Count - 1];
        }

        return null;
    }

    private static DataContainer LoadDataContainer()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<DataContainer>(json);
        }
        else
        {
            return new DataContainer(); //Return empty container if no file exists
        }
    }
}
