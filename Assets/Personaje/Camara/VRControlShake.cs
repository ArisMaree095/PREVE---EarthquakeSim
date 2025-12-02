using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class VRControlShake : MonoBehaviour
{
    private HapticImpulsePlayer hapticPlayer;

    [Header("Configuración Vibración")]
    public float fuerzaVibracion = 0.7f;
    public float duracionVibracion = 0.5f;

    void Start()
    {
        // Obtener o crear el componente háptico
        hapticPlayer = GetComponent<HapticImpulsePlayer>();
        if (hapticPlayer == null)
        {
            hapticPlayer = gameObject.AddComponent<HapticImpulsePlayer>();
        }

        // Suscribirse al evento del terremoto
        TerremotoManager.Terremoto += OnTerremotoOcurrido;
        Debug.Log($"Control {gameObject.name} listo para vibrar");
    }

    void OnTerremotoOcurrido(AnimationCurve curva, float duracion)
    {
        // Vibrar cuando ocurre el terremoto
        VibrarControl();
    }

    void VibrarControl()
    {
        if (hapticPlayer != null && hapticPlayer.enabled)
        {
            hapticPlayer.SendHapticImpulse(fuerzaVibracion, duracionVibracion);
            Debug.Log($"{gameObject.name} vibrando");
        }
    }

    void OnDestroy()
    {
        // Limpiar suscripción
        TerremotoManager.Terremoto -= OnTerremotoOcurrido;
    }
}