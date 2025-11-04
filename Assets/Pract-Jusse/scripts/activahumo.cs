using UnityEngine;

public class activahumo : MonoBehaviour
{
    
    public ParticleSystem humo; // Arrastra tu sistema de partículas aquí en el Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_prueba"))
        {
            if (!humo.isPlaying)
                humo.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player_prueba"))
        {
            if (humo.isPlaying)
                humo.Stop();
        }
    }
}

