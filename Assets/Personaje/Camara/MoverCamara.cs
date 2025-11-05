using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    public Transform PosicionCamara;

    private void Start()
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PosicionCamara.position;
    }
}
