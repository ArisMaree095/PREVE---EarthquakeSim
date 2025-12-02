using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SistemaMovimientoVR : MonoBehaviour
{
    [Header("XR References")]
    public XROrigin xrOrigin;
    public Transform orientacion; // Objeto vacío que marca la dirección del movimiento

    [Header("Input System")]
    public InputActionAsset contenedor;
    public InputActionReference movimiento; // Stick izquierdo - Movimiento
    public InputActionReference agachado;   // Botón para agacharse
    public InputActionReference movimientoCamara; // Nuevo: Stick derecho - Control de cámara

    [Header("Configuración Control de Cámara")]
    public Transform puntoRotacionCamara; // Pivot para rotar la cámara
    public float sensibilidadRotacionCamara = 2f;

    [Header("Configuración de movimiento")]
    public float velocidadCaminando = 2f;
    public float velocidadAgachado = 1f;
    public float SueloDrag = 3f;

    [Header("Agacharse")]
    [Range(0.3f, 0.8f)]
    public float alturaAgachado = 0.5f;
    public float distanciaRaycastAgachado = 1.0f;
    public float duracionAgachado = 0.3f;

    [Header("Detección de suelo")]
    public LayerMask capaSuelo;

    // Componentes
    private Rigidbody rb;

    // Variables de estado
    private bool enSuelo;
    private float agachadoLerp = 0f;
    private float alturaOriginal;

    public EstadoMovimiento estadoMovimiento;
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

        // Configurar orientación si no está asignada
        if (orientacion == null)
        {
            orientacion = new GameObject("OrientacionMovimiento").transform;
            orientacion.SetParent(transform);
        }

        // Configurar punto de rotación de cámara si no está asignado
        if (puntoRotacionCamara == null && xrOrigin != null)
        {
            puntoRotacionCamara = xrOrigin.transform;
        }
    }

    private void OnEnable()
    {
        // Habilitar todas las acciones usando el formato InputActionReference
        if (movimiento != null) movimiento.action.Enable();
        if (agachado != null) agachado.action.Enable();
        if (movimientoCamara != null) movimientoCamara.action.Enable();

        // Alternativa: habilitar el contenedor completo
        if (contenedor != null) contenedor.Enable();
    }

    private void OnDisable()
    {
        // Deshabilitar todas las acciones
        if (movimiento != null) movimiento.action.Disable();
        if (agachado != null) agachado.action.Disable();
        if (movimientoCamara != null) movimientoCamara.action.Disable();

        // Alternativa: deshabilitar el contenedor completo
        if (contenedor != null) contenedor.Disable();
    }

    void Update()
    {
        ActualizarOrientacion();
        DetectarSuelo();
        ManejadorDeEstado();
        InputAgachado();
        AplicarAgachadoXR();
        DragadoEnSuelo();
        MantenerEnSuelo();
        ControlarCamara(); // Control de cámara con InputActionReference
    }

    private void FixedUpdate()
    {
        MoverPersonajeVR();
        ControlDeVelocidad();
    }

    // CONTROL DE CÁMARA CON INPUTACTIONREFERENCE
    private void ControlarCamara()
    {
        if (movimientoCamara?.action == null || puntoRotacionCamara == null) return;

        Vector2 inputCamara = movimientoCamara.action.ReadValue<Vector2>();

        // Rotación horizontal con el eje X del stick
        if (Mathf.Abs(inputCamara.x) >= 0.1f)
        {
            float rotacionHorizontal = inputCamara.x * sensibilidadRotacionCamara * Time.deltaTime;
            puntoRotacionCamara.Rotate(0, rotacionHorizontal, 0, Space.World);
        }
    }

    // ACTUALIZAR ORIENTACIÓN BASADA EN LA CÁMARA VR
    private void ActualizarOrientacion()
    {
        if (xrOrigin?.Camera == null || orientacion == null) return;

        Vector3 cameraForward = xrOrigin.Camera.transform.forward;
        cameraForward.y = 0;

        if (cameraForward != Vector3.zero)
        {
            orientacion.rotation = Quaternion.LookRotation(cameraForward.normalized);
        }
    }

    // MOVIMIENTO CON INPUTACTIONREFERENCE
    private void MoverPersonajeVR()
    {
        if (movimiento?.action == null) return;

        Vector2 inputVR = movimiento.action.ReadValue<Vector2>();

        if (inputVR.magnitude >= 0.1f)
        {
            Vector3 direccion = orientacion.forward * inputVR.y + orientacion.right * inputVR.x;
            direccion.y = 0;

            Vector3 movimientoFisico = direccion.normalized * ObtenerVelocidadActual() * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movimientoFisico);
        }
    }

    // DETECCIÓN DE AGACHADO CON INPUTACTIONREFERENCE
    private void ManejadorDeEstado()
    {
        if (xrOrigin?.Camera == null) return;

        Vector3 origenRaycast = xrOrigin.Camera.transform.position;

        Debug.DrawRay(origenRaycast, Vector3.up * distanciaRaycastAgachado, Color.red);

        // Verificar agachado con InputActionReference
        bool agachando = agachado?.action?.ReadValue<float>() > 0.1f;

        if (agachando)
        {
            estadoMovimiento = EstadoMovimiento.agachado;
            return;
        }

        // Verificar obstáculos al levantarse
        if (Physics.Raycast(origenRaycast, Vector3.up, distanciaRaycastAgachado, capaSuelo))
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

        float targetHeight = (estadoMovimiento == EstadoMovimiento.agachado)
            ? alturaAgachado
            : alturaOriginal;

        xrOrigin.CameraYOffset = Mathf.Lerp(alturaOriginal, alturaAgachado, agachadoLerp);
    }

    // MÉTODOS AUXILIARES
    private float ObtenerVelocidadActual()
    {
        return Mathf.Lerp(velocidadCaminando, velocidadAgachado, agachadoLerp);
    }

    void DetectarSuelo()
    {
        enSuelo = Physics.Raycast(transform.position, Vector3.down, 1.5f, capaSuelo);
    }

    private void MantenerEnSuelo()
    {
        if (!enSuelo)
        {
            rb.AddForce(Vector3.down * 25f, ForceMode.Acceleration);
        }
    }

    void DragadoEnSuelo()
    {
        rb.linearDamping = enSuelo ? SueloDrag : 0f;
    }

    void ControlDeVelocidad()
    {
        Vector3 velocidadHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (velocidadHorizontal.magnitude > ObtenerVelocidadActual())
        {
            Vector3 velocidadLimitada = velocidadHorizontal.normalized * ObtenerVelocidadActual();
            rb.linearVelocity = new Vector3(velocidadLimitada.x, rb.linearVelocity.y, velocidadLimitada.z);
        }
    }

    // MÉTODO PARA DEBUG
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && xrOrigin?.Camera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(xrOrigin.Camera.transform.position, Vector3.up * distanciaRaycastAgachado);
        }
    }
}