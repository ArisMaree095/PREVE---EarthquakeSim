using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SistemaAgachadoVR : MonoBehaviour
{
    [Header("Input System")]
    public InputActionAsset contenedor;
    public InputActionReference agachado;

    [Header("Referencias XR")]
    public GameObject xrOrigin;
    public CharacterController characterController;

    [Header("Configuración Agachado")]
    [Range(0.0f, 0.5f)]
    public float alturaAgachado = 0.8f;
    public float duracionTransicion = 0.3f;
    public float distanciaRaycastAgachado = 1.0f;

    [Header("Configuración de Detección")]
    public LayerMask capaSuelo = 1; // Layer por defecto

    // Componentes XR
    private Camera xrCamera;
    private ActionBasedContinuousMoveProvider moveProvider;

    // Variables privadas
    private float alturaOriginal;
    private float agachadoLerp = 0f;
    private bool estaAgachado = false;

    // Para CharacterController
    private float alturaCharacterOriginal;
    private Vector3 centroCharacterOriginal;

    // Para CameraOffset
    private Transform cameraOffset;
    private Vector3 posicionOffsetOriginal;

    private void Start()
    {
        InicializarReferencias();
        GuardarValoresOriginales();

        // Debug para verificar la capa de suelo
        Debug.Log($"Capa Suelo configurada: {capaSuelo.value}");
    }

    private void InicializarReferencias()
    {
        // Buscar XR Origin
        if (xrOrigin == null)
        {
            xrOrigin = GameObject.Find("XR Origin");
            if (xrOrigin == null)
            {
                xrOrigin = GameObject.Find("XR Interaction Setup");
                if (xrOrigin == null)
                {
                    Debug.LogError("No se encontró XR Origin en la escena");
                    return;
                }
            }
        }

        // Buscar cámara
        xrCamera = xrOrigin.GetComponentInChildren<Camera>();
        if (xrCamera == null)
        {
            Debug.LogError("No se encontró Camera en el XR Origin");
            return;
        }

        // Buscar CharacterController
        if (characterController == null)
        {
            characterController = xrOrigin.GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
                if (characterController == null)
                {
                    Debug.LogError("No se encontró CharacterController");
                    return;
                }
            }
        }

        // Buscar o crear CameraOffset
        cameraOffset = xrCamera.transform.parent;
        if (cameraOffset == null || cameraOffset == xrOrigin.transform)
        {
            CrearCameraOffset();
        }

        // Buscar move provider para ajustar velocidad
        moveProvider = xrOrigin.GetComponent<ActionBasedContinuousMoveProvider>();

        Debug.Log("Referencias inicializadas correctamente");
    }

    private void CrearCameraOffset()
    {
        // Crear nuevo GameObject como padre de la cámara
        cameraOffset = new GameObject("CameraOffset").transform;
        cameraOffset.SetParent(xrOrigin.transform);
        cameraOffset.localPosition = Vector3.zero;
        cameraOffset.localRotation = Quaternion.identity;

        // Re-parent la cámara
        xrCamera.transform.SetParent(cameraOffset);
        Debug.Log("CameraOffset creado automáticamente");
    }

    private void GuardarValoresOriginales()
    {
        // Guardar posición original del offset
        if (cameraOffset != null)
        {
            posicionOffsetOriginal = cameraOffset.localPosition;
            alturaOriginal = posicionOffsetOriginal.y;
            Debug.Log($"Altura original guardada: {alturaOriginal}");
        }

        // Guardar valores del CharacterController
        alturaCharacterOriginal = characterController.height;
        centroCharacterOriginal = characterController.center;
        Debug.Log($"CharacterController original - Altura: {alturaCharacterOriginal}, Centro: {centroCharacterOriginal}");
    }

    private void OnEnable()
    {
        if (contenedor != null)
        {
            contenedor.Enable();
            Debug.Log("Input Action Asset habilitado");
        }

        if (agachado != null)
        {
            agachado.action.Enable();
            Debug.Log("Acción de agachado habilitada");
        }
    }

    private void OnDisable()
    {
        if (contenedor != null) contenedor.Disable();
        if (agachado != null) agachado.action.Disable();
    }

    void Update()
    {
        if (xrCamera == null || characterController == null || cameraOffset == null)
        {
            Debug.LogError("Referencias críticas no encontradas");
            return;
        }

        ManejarInputAgachado();
        ActualizarTransicionAgachado();
        AplicarAgachado();
    }

    private void ManejarInputAgachado()
    {
        if (agachado?.action == null)
        {
            Debug.LogWarning("Acción de agachado no asignada");
            return;
        }

        bool inputAgachado = agachado.action.ReadValue<float>() > 0.1f;

        if (inputAgachado && !estaAgachado)
        {
            Debug.Log("Iniciando agachado...");
            estaAgachado = true;
        }
        else if (!inputAgachado && estaAgachado)
        {
            if (PuedeLevantarse())
            {
                Debug.Log("Levantándose...");
                estaAgachado = false;
            }
            else
            {
                Debug.Log("No puede levantarse - obstáculo detectado");
            }
        }
    }

    private bool PuedeLevantarse()
    {
        if (xrCamera == null) return true;

        Vector3 origenRaycast = xrCamera.transform.position;
        RaycastHit hit;
        bool hayObstaculo = Physics.Raycast(origenRaycast, Vector3.up, out hit, distanciaRaycastAgachado, capaSuelo);

        // Debug visual
        Debug.DrawRay(origenRaycast, Vector3.up * distanciaRaycastAgachado, hayObstaculo ? Color.red : Color.green);

        if (hayObstaculo)
        {
            Debug.Log($"Obstáculo detectado: {hit.collider.gameObject.name} a distancia {hit.distance:F2}");
        }

        return !hayObstaculo;
    }

    private void ActualizarTransicionAgachado()
    {
        float targetLerp = estaAgachado ? 1f : 0f;

        float velocidadTransicion = 1f / duracionTransicion;

        agachadoLerp = Mathf.MoveTowards(agachadoLerp, targetLerp, velocidadTransicion * Time.deltaTime);

        // Debug ocasional del progreso
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Progreso agachado: {agachadoLerp:F2}");
        }
    }

    private void AplicarAgachado()
    {
        AplicarAgachadoCameraOffset();
        AplicarAgachadoCharacterController();
        AplicarEfectosAgachado();
    }

    private void AplicarAgachadoCameraOffset()
    {
        // Mover el CameraOffset en Y
        float alturaActual = Mathf.Lerp(alturaOriginal, alturaAgachado, agachadoLerp);

        Vector3 nuevaPosicion = new Vector3(
            posicionOffsetOriginal.x,
            alturaActual,
            posicionOffsetOriginal.z
        );

        cameraOffset.localPosition = nuevaPosicion;

        // Debug de posición
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"CameraOffset Y: {cameraOffset.localPosition.y:F2}");
        }
    }

    private void AplicarAgachadoCharacterController()
    {
        // Ajustar CharacterController
        float alturaActual = Mathf.Lerp(alturaCharacterOriginal, alturaAgachado, agachadoLerp);
        characterController.height = alturaActual;

        // Calcular diferencia de altura para ajustar el centro
        float diferenciaAltura = (alturaCharacterOriginal - alturaActual);
        Vector3 centroActual = new Vector3(
            centroCharacterOriginal.x,
            centroCharacterOriginal.y - (diferenciaAltura / 2f),
            centroCharacterOriginal.z
        );
        characterController.center = centroActual;
    }

    private void AplicarEfectosAgachado()
    {
        // Ajustar velocidad del movimiento si existe el move provider
        if (moveProvider != null)
        {
            float velocidadActual = Mathf.Lerp(3f, 1.5f, agachadoLerp);
            moveProvider.moveSpeed = velocidadActual;
        }
    }

    // Métodos públicos para integración con otros sistemas
    public void ForzarAgachado(bool agachar)
    {
        if (agachar)
        {
            estaAgachado = true;
            Debug.Log("Agachado forzado activado");
        }
        else
        {
            if (PuedeLevantarse())
            {
                estaAgachado = false;
                Debug.Log("Agachado forzado desactivado");
            }
            else
            {
                Debug.LogWarning("No se puede desforzar agachado - obstáculo detectado");
            }
        }
    }

    public bool EstaAgachado()
    {
        return estaAgachado;
    }

    public float GetProgresoAgachado()
    {
        return agachadoLerp;
    }

    public bool PuedeLevantarseAhora()
    {
        return PuedeLevantarse();
    }

    // Método para resetear a estado original
    public void ResetearAgachado()
    {
        estaAgachado = false;
        agachadoLerp = 0f;
        AplicarAgachado();
        Debug.Log("Agachado reseteado");
    }

    // Debug en pantalla
    private void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(10, 100, 400, 30), $"Agachado: {estaAgachado} | Progreso: {agachadoLerp:F2}", style);
            GUI.Label(new Rect(10, 130, 400, 30), $"Offset Y: {cameraOffset?.localPosition.y:F2}", style);
            GUI.Label(new Rect(10, 160, 400, 30), $"Puede Levantarse: {PuedeLevantarse()}", style);
            GUI.Label(new Rect(10, 190, 400, 30), $"Capa Suelo: {capaSuelo.value}", style);
        }
    }
}