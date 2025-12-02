using UnityEngine;

public class TerremotoManager : MonoBehaviour
{
    public static event System.Action<AnimationCurve, float> Terremoto;

    public static void EmitirTerremoto(AnimationCurve curva, float duracion)
    {
        Terremoto?.Invoke(curva, duracion);
    }


}
