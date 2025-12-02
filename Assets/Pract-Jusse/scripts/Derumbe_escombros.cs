using UnityEngine;

public class Derumbe_escombros : MonoBehaviour
{
    private Fracture fractureScript;

    private void Start()
    {
        fractureScript = GetComponent<Fracture>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (fractureScript != null)
        {
            fractureScript.FractureObject();
        }
    }
}