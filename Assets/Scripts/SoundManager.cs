using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    
    public void PlayDestroyAudioClip()
    {
        _audioSource.Play();
    }
}
