using UnityEngine;

public class Persprueba : MonoBehaviour
{
    public float velocidad = 3f; // velocidad de movimiento
    public Transform rig; // el GameObject del XR Rig (cámara + controladores)

    void Update()
    {
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector3 direccion = rig.forward * z + rig.right * x;
        direccion.y = 0; // mantener altura fija
        rig.position += direccion * velocidad * Time.deltaTime;
    }
}