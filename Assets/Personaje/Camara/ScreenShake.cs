using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public Transform CamaraPosition;

    void Start()
    {
        // Suscribirse al evento con parámetros
        TerremotoManager.Terremoto += OnTerremotoOcurrido;
    }

    void OnTerremotoOcurrido(AnimationCurve curva, float duracion)
    {
        // Usar la curva y duración recibidas del trigger
        StartCoroutine(Shaking(curva, duracion));
    }

    IEnumerator Shaking(AnimationCurve curva, float duracion)
    {
        Vector3 originalPos = CamaraPosition.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duracion)
        {
            float fuerza = curva.Evaluate(elapsed / duracion);
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
        TerremotoManager.Terremoto -= OnTerremotoOcurrido;
    }
}