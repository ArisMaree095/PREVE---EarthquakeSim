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

       
        VerRaycast();
    }

    
    

    private void FixedUpdate()
    {
        MoverPersonaje();
    }

    private void ManejadorDeEstado()
    {
        var capsule = colisionadorJugador as CapsuleCollider;
        if (capsule == null)
            return;

        // Si el botón de agachado está presionado, siempre agachado
        if (agachado.action.IsPressed())
        {
            estadoMovimiento = EstadoMovimiento.agachado;
            return;
        }

        // Solo realiza el raycast si está agachado
        if (estadoMovimiento == EstadoMovimiento.agachado)
        {
            float distancia = distanciaRaycastAgachado;
            Vector3 origen = transform.position + Vector3.up * (capsule.height / 2f);

            // Si el raycast detecta un obstáculo, no hacer nada (mantener agachado)
            if (Physics.Raycast(origen, Vector3.up, distancia))
            {
                // No hacer nada, permanece agachado
                return;
            }
            else
            {
                // Si no detecta obstáculo, cambiar a modo caminar
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

        if (estadoMovimiento == EstadoMovimiento.agachado)
        {
            velocidad = velocidadAgachado;
            capsule.height = alturaAgachado;
            float diferencia = (alturaColliderInicial - alturaAgachado) / 2f;
            capsule.center = new Vector3(centroColliderInicial.x, centroColliderInicial.y - diferencia, centroColliderInicial.z);
        }
        else // Estado caminando
        {
            velocidad = 5f;
            capsule.height = alturaColliderInicial;
            capsule.center = centroColliderInicial;
        }
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


    void VerRaycast()
    {
        // Visualización del raycast hacia arriba (agachado) usando Debug.DrawRay
        var capsule = colisionadorJugador as CapsuleCollider;
        if (capsule != null && estadoMovimiento == EstadoMovimiento.agachado)
        {
            Vector3 origenArriba = transform.position + Vector3.up * (capsule.height / 2f);
            Vector3 direccionArriba = Vector3.up;
            float distanciaArriba = distanciaRaycastAgachado;

            bool hayObstaculo = Physics.Raycast(origenArriba, direccionArriba, distanciaArriba);
            Color colorRay = hayObstaculo ? Color.green : Color.red;
            Debug.DrawRay(origenArriba, direccionArriba * distanciaArriba, colorRay);
        }
    }


    private void OnDrawGizmos()
    {
        // Raycast hacia abajo (suelo)
        Gizmos.color = Color.green;
        Vector3 origenSuelo = transform.position;
        Vector3 destinoSuelo = origenSuelo + Vector3.down * alturaJugador;
        Gizmos.DrawLine(origenSuelo, destinoSuelo);
        Gizmos.DrawSphere(destinoSuelo, 0.05f);

        // Raycast hacia arriba (agachado)
        var capsule = colisionadorJugador as CapsuleCollider;
        if (capsule != null)
        {
            Gizmos.color = Color.red;
            // Antes:
            // float distanciaArriba = (alturaColliderInicial - alturaAgachado) / 2f + capsule.height / 2f + 0.05f;

            // Después:
            float distanciaArriba = distanciaRaycastAgachado;
            Vector3 origenArriba = transform.position + Vector3.up * (capsule.height / 2f);
            Vector3 destinoArriba = origenArriba + Vector3.up * distanciaArriba;
            Gizmos.DrawLine(origenArriba, destinoArriba);
            Gizmos.DrawSphere(destinoArriba, capsule.radius);
        }
    }
}

