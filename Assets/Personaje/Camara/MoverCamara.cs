using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    public Transform PosicionCamara;
    public SistemaMovimiento SistemaMovimiento;

    private void Start()
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        EstadoDeCamara();
    }

    void EstadoDeCamara()
    {
        if (SistemaMovimiento.estadoMovimiento == SistemaMovimiento.EstadoMovimiento.agachado)
        {
            Vector3 targetPosition = PosicionCamara.position + new Vector3(0, -SistemaMovimiento.alturaAgachado, 0);
            float newY = Mathf.Lerp(transform.position.y, targetPosition.y, Time.deltaTime * 10);
            transform.position = new Vector3(PosicionCamara.position.x, newY, PosicionCamara.position.z);
        }
        else
        {
            float newY = Mathf.Lerp(transform.position.y, PosicionCamara.position.y, Time.deltaTime * 10);
            transform.position = new Vector3(PosicionCamara.position.x, newY, PosicionCamara.position.z);
        }
    }
}
