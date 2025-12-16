using UnityEngine;

public class OpenManual : MonoBehaviour
{
    public string manualURL = "https://drive.google.com/drive/u/0/folders/1kKw6VSXWsfhROXaxrJD-2aKX9r7yeqEo";

    public void OpenLink()
    {
        Application.OpenURL(manualURL);
    }
}
