using UnityEngine;

public class RaycastPrueba : MonoBehaviour
{
    public float distancia = 5f;

    void Update()
    {
        Vector3 origen = transform.position;
        Vector3 direccion = Vector3.up;

        if (Physics.Raycast(origen, direccion, out RaycastHit hit, distancia))
        {
            Debug.Log("Colisión con: " + hit.collider.name);
        }

        // Visualización en la escena
        Debug.DrawRay(origen, direccion * distancia, Color.red);
    }
}
