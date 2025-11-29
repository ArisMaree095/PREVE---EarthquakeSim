using UnityEngine;
using System.Collections;

public class SonidoTerremoto : MonoBehaviour
{
    public AudioClip sonidoTerremoto;
    public float duracionAudio = 5f; // Duración total del audio
    public float tiempoFadeOut = 1f; // Tiempo para el fade out
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    void Start()
    {
        // Obtener o crear AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurar el AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // Suscribirse al evento
        TerremotoManager.Terremoto += ReproducirSonidoTerremoto;
    }

    void ReproducirSonidoTerremoto()
    {
        if (sonidoTerremoto != null && audioSource != null)
        {
            // Detener corrutina anterior si existe
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            // Configurar y reproducir
            audioSource.volume = 1f;
            audioSource.clip = sonidoTerremoto;
            audioSource.Play();

            // Iniciar fade out automático
            fadeCoroutine = StartCoroutine(FadeOutAudio());
            Debug.Log("🔊 Sonido de terremoto reproducido");
        }
        else
        {
            Debug.LogWarning("❌ AudioClip o AudioSource no configurado");
        }
    }

    IEnumerator FadeOutAudio()
    {
        // Esperar hasta que sea momento de empezar el fade out
        float tiempoEspera = duracionAudio - tiempoFadeOut;
        yield return new WaitForSeconds(tiempoEspera);

        // Aplicar fade out
        float tiempoTranscurrido = 0f;
        float volumenInicial = audioSource.volume;

        while (tiempoTranscurrido < tiempoFadeOut)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / tiempoFadeOut;
            audioSource.volume = Mathf.Lerp(volumenInicial, 0f, progreso);
            yield return null;
        }

        // Asegurar que el volumen llegue a 0 y detener el audio
        audioSource.volume = 0f;
        audioSource.Stop();

        Debug.Log("🔇 Audio fade out completado");
    }

    void OnDestroy()
    {
        // Importante: Desuscribirse al destruir el objeto
        TerremotoManager.Terremoto -= ReproducirSonidoTerremoto;

        // Detener corrutinas
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
    }
}