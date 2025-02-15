using UnityEngine;
using Zenject;

public class Menu : MonoBehaviour
{
    [Inject]
    private void Inject(AudioSource audioSource)
    {
        if(!audioSource.isPlaying)
            audioSource.Play();
        Time.timeScale = 1;
    }
}
