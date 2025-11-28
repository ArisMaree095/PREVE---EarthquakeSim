using UnityEngine;

public class animationes : MonoBehaviour
{
   
    public Animator animator;        
    public string animationName;    

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player_prueba"))
        {
            animator.Play(animationName);
        }
    }
}

