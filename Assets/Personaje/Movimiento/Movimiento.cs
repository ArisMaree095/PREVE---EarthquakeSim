using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento : MonoBehaviour
{
    [Header("Velocidad de movimiento del personaje")]
    public float velocidad = 5f;
    public float SueloDrag;

    [Header("Revisar si esta en suelo")]
    public float alturaJugador;
    public LayerMask capaSuelo;
    private bool enSuelo;

    public Transform orientacion;
    public InputActionReference movimientoInput;

    private Rigidbody rb;
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evitar que el Rigidbody rote por físicas

        // Habilitar la acción de movimiento
        if (movimientoInput != null)
        {
            movimientoInput.action.Enable();
        }
    }

    private void Update()
    {
        enSuelo = Physics.Raycast(transform.position, Vector3.down, alturaJugador, capaSuelo);
        rb.linearDamping = enSuelo ? SueloDrag : 0;

        MiInput();
        direccionMovimiento = orientacion.forward * movimientoVertical + orientacion.right * movimientoHorizontal;
    }

    private void FixedUpdate()
    {
        MoverPersonaje();
    }

    public void MiInput()
    {
        if (movimientoInput != null)
        {
            Vector2 input = movimientoInput.action.ReadValue<Vector2>();
            movimientoHorizontal = input.x;
            movimientoVertical = input.y;
        }
        else
        {
            movimientoHorizontal = 0f;
            movimientoVertical = 0f;
        }
    }

    private void MoverPersonaje()
    {
        rb.MovePosition(rb.position + direccionMovimiento.normalized * velocidad * Time.fixedDeltaTime);
    }
}
