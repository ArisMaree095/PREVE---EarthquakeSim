using UnityEngine;

public class TerremotoTrigger : MonoBehaviour
{
    public AnimationCurve curvaShake = AnimationCurve.Linear(0, 1, 1, 0);
    public float duracionShake = 0.5f;
    private bool yaActivado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (yaActivado) return;

        if (other.CompareTag("Player_prueba"))
        {
            yaActivado = true;
            TerremotoManager.EmitirTerremoto(curvaShake, duracionShake);
        }
    }
}