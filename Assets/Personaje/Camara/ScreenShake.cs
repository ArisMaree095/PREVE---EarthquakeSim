using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public float duration = 0.5f; // Duration of the shake
    public AnimationCurve curva;
    public Transform CamaraPosition;

    // Eliminamos shouldShake ya que lo controlará el evento

    void Start()
    {
        // Suscribirse al evento del terremoto
        TerremotoManager.Terremoto += OnTerremotoOcurrido;
    }

    void OnTerremotoOcurrido()
    {
        // Iniciar el screen shake cuando ocurra el terremoto
        StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        Vector3 originalPos = CamaraPosition.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float fuerza = curva.Evaluate(elapsed / duration);
            float x = Random.Range(-1f, 1f) * fuerza;
            float y = Random.Range(-1f, 1f) * fuerza;
            CamaraPosition.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
        CamaraPosition.localPosition = originalPos;
    }

    void OnDestroy()
    {
        // Importante: Desuscribirse al destruir el objeto
        TerremotoManager.Terremoto -= OnTerremotoOcurrido;
    }
}