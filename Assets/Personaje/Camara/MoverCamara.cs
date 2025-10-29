using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    public Transform PosicionCamara;

    // Update is called once per frame
    void Update()
    {
        transform.position = PosicionCamara.position;
    }
}
