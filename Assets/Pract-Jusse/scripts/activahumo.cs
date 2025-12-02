using UnityEngine;

public class activahumo : MonoBehaviour
{
    public GameObject objeto; 

    private bool activado = false;

    private void Start()
    {
        if (objeto != null)
            objeto.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activado && other.CompareTag("Player_prueba"))
        {
            if (objeto != null)
                objeto.SetActive(true);

            activado = true; 


        }
    }
}

