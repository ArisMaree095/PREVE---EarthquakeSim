using UnityEngine;
using UnityEngine.InputSystem;

public class SistemaMovimiento : MonoBehaviour
{
    [Header("Input System")]
    // Componente maestro, que posee todos los mapas de acción
    public InputActionAsset contenedor;
    // Acciones individuales
    public InputActionReference movimiento;
    public InputActionReference agachado;


    [Header("Velocidad de movimiento del personaje")]
    public float velocidad = 5f;
    // El drag no se siente
    public float SueloDrag;


    [Header("Revisar si esta en suelo")]
    [Range(0.5f, 1.0f)]
    public float alturaJugador;
    public LayerMask capaSuelo;
    private bool enSuelo;


    [Header("Agacharse")]
    public float velocidadAgachado;
    [Range(0.5f, 1.0f)]
    public float alturaAgachado;
    [Tooltip("Distancia máxima del raycast hacia arriba al agacharse")]
    public float distanciaRaycastAgachado = 1.0f;



    [Header("Orientación")]
    public Transform orientacion;
    public Collider colisionadorJugador;


    private Rigidbody rb;

    private float movimientoHorizontal;
    private float movimientoVertical;

    private Vector3 direccionMovimiento;

    public EstadoMovimiento estadoMovimiento;
    private float alturaColliderInicial;
    private Vector3 centroColliderInicial;
    private float agachadoLerp = 0f;
    public float duracionAgachado = 0.3f; // Puedes ajustar este valor en el inspector

    public Transform PosicionCamara;

    public enum EstadoMovimiento { caminando, agachado }


    private void OnEnable()
    {
        contenedor.Enable();
        print("Habilitado");
    }



    private void OnDisable()
    {
        contenedor.Disable();
        print("Deshabilitado");
    }


   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Guarda valores originales del collider
        var capsule = colisionadorJugador as CapsuleCollider;
        if (capsule != null)
        {
            alturaColliderInicial = capsule.height;
            centroColliderInicial = capsule.center;
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

    
    }

    
    

    private void FixedUpdate()
    {
        MoverPersonaje();
    }

    private void ManejadorDeEstado()
    {
        // revisa la capsula
        var capsule = colisionadorJugador as CapsuleCollider;
        if (capsule == null)
            return;

        // valores del raycast
        float distancia = distanciaRaycastAgachado;
        Vector3 origen = transform.position;
        origen.y -= 1;

        Debug.DrawRay(origen, Vector3.up * distancia, Color.red);

        // Si el botón de agachado está presionado, siempre agachado
        if (agachado.action.IsPressed())
        {
            estadoMovimiento = EstadoMovimiento.agachado;
            return;
        }
        

        // El raycast se ejecuta al no agacharse, pero se dibuja siempre
        if (Physics.Raycast(origen, Vector3.up, distancia))
        {
           
            if (estadoMovimiento == EstadoMovimiento.agachado)
            {
                estadoMovimiento = EstadoMovimiento.agachado;
                return;
            }
        }

        else
        {
            if (estadoMovimiento == EstadoMovimiento.agachado)
            {
                estadoMovimiento = EstadoMovimiento.caminando;
                return;
            }
        }
        
        // Si no está agachado y no se presiona el botón, puede caminar
        estadoMovimiento = EstadoMovimiento.caminando;
    }

    

    void DetectarSuelo()
    {
        enSuelo = Physics.Raycast(transform.position, Vector3.down, alturaJugador, capaSuelo);
    }

    private void MantenerEnSuelo()
    {
        if (!enSuelo)
        {
            // Aplica una fuerza extra hacia abajo si no está en el suelo
            rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
        }
    }

    void DragadoEnSuelo()
    {
        enSuelo = Physics.Raycast(transform.position, Vector3.down, alturaJugador, capaSuelo);

        if (enSuelo)
        {
            rb.linearDamping = SueloDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }


    private void MoverPersonaje()
    {
        rb.MovePosition(rb.position + direccionMovimiento.normalized * velocidad * Time.fixedDeltaTime);
    }

    private void DireccionMovimiento()
    {
        direccionMovimiento = orientacion.forward * movimientoVertical + orientacion.right * movimientoHorizontal;

    }

    private void InputDeMovimiento()
    {
        if (movimiento.action.ReadValue<Vector2>().magnitude >= 0.5f)
        {
            print("Moviendo");
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



    private void InputAgachado()
    {
        var capsule = colisionadorJugador as CapsuleCollider;
        if (capsule == null)
            return;

        // Determina los valores objetivo
        float alturaObjetivo;
        Vector3 centroObjetivo;

        if (estadoMovimiento == EstadoMovimiento.agachado)
        {
            alturaObjetivo = alturaAgachado;
            float diferencia = (alturaColliderInicial - alturaAgachado) / 2f;
            centroObjetivo = new Vector3(centroColliderInicial.x, centroColliderInicial.y - diferencia, centroColliderInicial.z);

            // Avanza la interpolación
            agachadoLerp += Time.deltaTime / duracionAgachado;
        }
        else // Estado caminando
        {
            alturaObjetivo = alturaColliderInicial;
            centroObjetivo = centroColliderInicial;

            // Retrocede la interpolación
            agachadoLerp -= Time.deltaTime / duracionAgachado;
        }

        // Limita el progreso entre 0 y 1
        agachadoLerp = Mathf.Clamp01(agachadoLerp);

        // Interpola altura y centro
        capsule.height = Mathf.Lerp(alturaColliderInicial, alturaAgachado, agachadoLerp);
        capsule.center = Vector3.Lerp(centroColliderInicial,
            new Vector3(centroColliderInicial.x, centroColliderInicial.y - ((alturaColliderInicial - alturaAgachado) / 2f), centroColliderInicial.z),
            agachadoLerp);

        // Interpola velocidad
        velocidad = Mathf.Lerp(5f, velocidadAgachado, agachadoLerp);
    }


    void ControlDeVelocidad()
    {
        Vector3 velocidadHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (velocidadHorizontal.magnitude > velocidad)
        {
            Vector3 velocidadLimitada = velocidadHorizontal.normalized * velocidad;
            rb.linearVelocity = new Vector3(velocidadLimitada.x, rb.linearVelocity.y, velocidadLimitada.z);
        }
    }


   
}

