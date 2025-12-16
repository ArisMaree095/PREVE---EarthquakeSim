using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SistemaMovimiento : MonoBehaviour
{
    [Header("XR References")]
    public XROrigin xrOrigin;

    [Header("Input System")]
    public InputActionAsset contenedor;
    public InputActionReference movimiento;
    public InputActionReference agachado;

    [Header("Configuración de movimiento")]
    public float velocidadCaminando = 5f;
    public float velocidadAgachado = 2f;
    public float SueloDrag;

    [Header("Orientación")]
    public Transform orientacion;

    [Header("Agacharse")]
    [Range(0.5f, 1.0f)]
    public float alturaAgachado = 0.5f;
    public float distanciaRaycastAgachado = 1.0f;
    public float duracionAgachado = 0.3f;

    [Header("Detección de suelo")]
    public LayerMask capaSuelo;
    private bool enSuelo;

    // Variables de movimiento manual
    private Rigidbody rb;
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;

    // Variables de agachado
    private float agachadoLerp = 0f;
    public EstadoMovimiento estadoMovimiento;
    private float alturaOriginal;

    public enum EstadoMovimiento { caminando, agachado }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Guardar altura original del XR Origin
        if (xrOrigin != null)
        {
            alturaOriginal = xrOrigin.CameraYOffset;
        }
    }

    void Update()
    {
        DetectarSuelo();
        InputDeMovimiento();
        ManejadorDeEstado();
        InputAgachado();
        ControlDeVelocidad();
        DireccionMovimiento();
        DragadoEnSuelo();
        MantenerEnSuelo();
        AplicarAgachadoXR();
    }

    private void FixedUpdate()
    {
        MoverPersonaje();
    }

    // MÉTODOS DE MOVIMIENTO MANUAL (MANTENIDOS)
    private void MoverPersonaje()
    {
        rb.MovePosition(rb.position + direccionMovimiento.normalized * velocidadCaminando * Time.fixedDeltaTime);
    }

    private void DireccionMovimiento()
    {
        direccionMovimiento = orientacion.forward * movimientoVertical + orientacion.right * movimientoHorizontal;
    }

    private void InputDeMovimiento()
    {
        if (movimiento.action.ReadValue<Vector2>().magnitude >= 0.5f)
        {
            Vector2 input = movimiento.action.ReadValue<Vector2>();
            movimientoHorizontal = input.x;
            movimientoVertical = input.y;
        }
        else
        {
            movimientoHorizontal = 0f;
            movimientoVertical = 0f;
        }
    }

    void DetectarSuelo()
    {
        enSuelo = Physics.Raycast(transform.position, Vector3.down, 1.5f, capaSuelo);
    }

    private void MantenerEnSuelo()
    {
        if (!enSuelo)
        {
            rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
        }
    }

    void DragadoEnSuelo()
    {
        if (enSuelo)
        {
            rb.linearDamping = SueloDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    void ControlDeVelocidad()
    {
        Vector3 velocidadHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (velocidadHorizontal.magnitude > velocidadCaminando)
        {
            Vector3 velocidadLimitada = velocidadHorizontal.normalized * velocidadCaminando;
            rb.linearVelocity = new Vector3(velocidadLimitada.x, rb.linearVelocity.y, velocidadLimitada.z);
        }
    }

    // MÉTODOS DE AGACHADO ADAPTADOS A XR
    private void ManejadorDeEstado()
    {
        if (xrOrigin?.Camera == null) return;

        Vector3 origenRaycast = xrOrigin.Camera.transform.position;

        Debug.DrawRay(origenRaycast, Vector3.up * distanciaRaycastAgachado, Color.red);

        if (agachado.action.IsPressed())
        {
            estadoMovimiento = EstadoMovimiento.agachado;
            return;
        }

        if (Physics.Raycast(origenRaycast, Vector3.up, distanciaRaycastAgachado))
        {
            if (estadoMovimiento == EstadoMovimiento.agachado)
            {
                estadoMovimiento = EstadoMovimiento.agachado;
                return;
            }
        }
        else
        {
            estadoMovimiento = EstadoMovimiento.caminando;
        }
    }

    private void InputAgachado()
    {
        if (estadoMovimiento == EstadoMovimiento.agachado)
        {
            agachadoLerp += Time.deltaTime / duracionAgachado;
        }
        else
        {
            agachadoLerp -= Time.deltaTime / duracionAgachado;
        }
        agachadoLerp = Mathf.Clamp01(agachadoLerp);
    }

    private void AplicarAgachadoXR()
    {
        if (xrOrigin == null) return;

        // Interpolar la altura de la cámara
        float targetHeight = (estadoMovimiento == EstadoMovimiento.agachado)
            ? alturaAgachado
            : alturaOriginal;

        xrOrigin.CameraYOffset = Mathf.Lerp(alturaOriginal, alturaAgachado, agachadoLerp);

        // Ajustar velocidad según estado
        velocidadCaminando = Mathf.Lerp(velocidadCaminando,
            (estadoMovimiento == EstadoMovimiento.agachado) ? velocidadAgachado : velocidadCaminando,
            agachadoLerp);
    }

    private void OnEnable()
    {
        contenedor?.Enable();
    }

    private void OnDisable()
    {
        contenedor?.Disable();
    }
}