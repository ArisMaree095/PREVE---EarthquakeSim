using UnityEngine;
using UnityEngine.UI;

public class IniciarLogic : MonoBehaviour
{
    public int sceneIndexToLoad = 1;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        //Singleton transition manager
        if (SceneTransitionManager.singleton != null)
            SceneTransitionManager.singleton.GoToSceneAsync(sceneIndexToLoad);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndexToLoad);
    }
}
