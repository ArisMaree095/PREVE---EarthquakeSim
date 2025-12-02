using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoverCamara : MonoBehaviour
{
    public XROrigin xrOrigin;
    public SistemaMovimiento sistemaMovimiento;

    private float alturaOriginal;
    private float alturaObjetivo;

    private void Start()
    {
        if (xrOrigin != null)
        {
            alturaOriginal = xrOrigin.CameraYOffset;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        EstadoDeCamara();
    }

    void EstadoDeCamara()
    {
        if (xrOrigin == null || sistemaMovimiento == null) return;

        // Determinar altura objetivo
        if (sistemaMovimiento.estadoMovimiento == SistemaMovimiento.EstadoMovimiento.agachado)
        {
            alturaObjetivo = alturaOriginal - sistemaMovimiento.alturaAgachado;
        }
        else
        {
            alturaObjetivo = alturaOriginal;
        }

        // Aplicar interpolación suave
        float nuevaAltura = Mathf.Lerp(xrOrigin.CameraYOffset, alturaObjetivo, Time.deltaTime * 10f);
        xrOrigin.CameraYOffset = nuevaAltura;
    }
}