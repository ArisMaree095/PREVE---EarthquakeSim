using Meta.XR.BuildingBlocks;
using UnityEngine;

public class TerremotoTrigger : MonoBehaviour
{
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_prueba"))
        {
            print("Entro al Trigger Terremoto");

            TerremotoManager.EmitirTerremoto();
        }
    }

}
