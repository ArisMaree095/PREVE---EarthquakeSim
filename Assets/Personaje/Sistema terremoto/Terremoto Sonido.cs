using UnityEngine;
using System.Collections;

public class SonidoTerremoto : MonoBehaviour
{
    public AudioClip sonidoTerremoto;
    public float duracionAudio = 5f;
    public float tiempoFadeOut = 1f;
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        TerremotoManager.Terremoto += OnTerremotoRecibido;
    }

    void OnTerremotoRecibido(AnimationCurve curva, float duracion)
    {
        ReproducirSonidoTerremoto();
    }

    void ReproducirSonidoTerremoto()
    {
        if (sonidoTerremoto != null && audioSource != null)
        {
            // Detener corrutina y audio anterior
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.volume = 1f;
            audioSource.clip = sonidoTerremoto;
            audioSource.Play();

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
        float tiempoEspera = duracionAudio - tiempoFadeOut;
        yield return new WaitForSeconds(tiempoEspera);

        float tiempoTranscurrido = 0f;
        float volumenInicial = audioSource.volume;

        while (tiempoTranscurrido < tiempoFadeOut)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / tiempoFadeOut;
            audioSource.volume = Mathf.Lerp(volumenInicial, 0f, progreso);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    void OnDestroy()
    {
        TerremotoManager.Terremoto -= OnTerremotoRecibido;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
    }
}