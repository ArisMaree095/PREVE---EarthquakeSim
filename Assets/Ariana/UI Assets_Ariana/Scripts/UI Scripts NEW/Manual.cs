using UnityEngine;

public class Manual : MonoBehaviour
{
    public string manualURL = "https://www.google.com";

    public void OpenLink()
    {
        Application.OpenURL(manualURL);
    }
}
