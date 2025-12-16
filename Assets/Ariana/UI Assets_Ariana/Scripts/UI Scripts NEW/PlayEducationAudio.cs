using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayEducationAudio : MonoBehaviour
{
    public AudioClip audioClip;

    void Start()
    {
        AudioSource audioClip = GetComponent<AudioSource>();
        audioClip.Play();
    }

    

    void Update()
    {
        
    }
}
