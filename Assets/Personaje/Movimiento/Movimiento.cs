using UnityEngine;

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

    private Rigidbody rb;
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evitar que el Rigidbody rote por físicas
    }


    private void Update()
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
        
        MiInput();
        direccionMovimiento = orientacion.forward * movimientoVertical + orientacion.right * movimientoHorizontal;
    }


    private void FixedUpdate()
    {
        MoverPersonaje();
    }


    private void MiInput()
    {
        movimientoHorizontal = Input.GetAxis("Horizontal");
        movimientoVertical = Input.GetAxis("Vertical");
    }
    

    private void MoverPersonaje()
    {
        rb.MovePosition(rb.position + direccionMovimiento.normalized * velocidad * Time.fixedDeltaTime);
    }
}
