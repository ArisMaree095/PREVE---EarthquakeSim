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
    public float SueloDrag;


    [Header("Revisar si esta en suelo")]
    public float alturaJugador;
    public LayerMask capaSuelo;
    private bool enSuelo;

    [Header("Agacharse")]
    public float velocidadAgachado;
    public float alturaAgachado;
    private float alturaInicial;

    [Header("Orientación")]
    public Transform orientacion;

    private Rigidbody rb;

    private float movimientoHorizontal;
    private float movimientoVertical;

    private Vector3 direccionMovimiento;


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
        rb.freezeRotation = true; // Evitar que el Rigidbody rote por físicas

    }

    


    void Update()
    {
        enSuelo = Physics.Raycast(transform.position, Vector3.down, alturaJugador, capaSuelo);
        

        InputDeMovimiento();
        InputAgachado();
        ControlDeVelocidad();
        DireccionMovimiento();
        DragadoEnSuelo();
    }



    private void FixedUpdate()
    {
        MoverPersonaje();
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
        if (agachado.action.IsPressed())
        {
            velocidad = velocidadAgachado;
            transform.localScale = new Vector3(transform.localScale.x, alturaAgachado, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else
        {
            velocidad = 5f;
            transform.localScale = new Vector3(transform.localScale.x, alturaInicial, transform.localScale.z);
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

