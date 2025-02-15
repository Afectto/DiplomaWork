using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    private class KeyValue
    {
        public SoundType Key;
        public AudioClip Value;
    }
    [SerializeField]
    private List<KeyValue> _clipsL;
    private static Dictionary<SoundType, AudioClip> _clipsD;
    private static AudioSource _audio;

    [Inject]
    private void Inject()
    {
        _clipsD = _clipsL.ToDictionary(item => item.Key, item => item.Value);
        _audio = GetComponent<AudioSource>();
    }

    public static void Play(SoundType type)
    {
        if(_audio == null || _clipsD == null || !_clipsD.ContainsKey(type))
        {
            Debug.Log("NO AUDIO CLIP");
            return;
        }
        _audio.PlayOneShot(_clipsD[type]);
    }
}
