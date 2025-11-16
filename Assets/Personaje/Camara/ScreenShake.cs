using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public float duration = 0.5f; // Duration of the shake
    public AnimationCurve curva;
    public Transform CamaraPosition;
    public bool shouldShake = false;



    // Update is called once per frame
    void Update()
    {
        if (shouldShake)
        {
            StartCoroutine(Shaking());
            shouldShake = false;
        }
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
}
