using UnityEngine;

public class TerremotoManager : MonoBehaviour
{
    public static event System.Action Terremoto;

    public float intensidad;
    public float tiempoDuracion;

    public static void EmitirTerremoto()
    {
        Terremoto?.Invoke();
    }

    
}
