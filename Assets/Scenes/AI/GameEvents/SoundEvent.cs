using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSoundEvent", menuName = "Audio/Sound Event")]
public class SoundEvent : ScriptableObject
{
    public string eventName;
    
    [Header("Lista de sunete (se vor reda in ordine)")]
    public List<AudioClip> clips; 
    
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    private int nextClipIndex = 0;

    // Metodă care ne dă următorul clip din listă
    public AudioClip GetNextClip()
    {
        if (clips == null || clips.Count == 0) return null;

        AudioClip clipToPlay = clips[nextClipIndex];
        
        // Trecem la următorul index (și revenim la 0 dacă am ajuns la capăt)
        nextClipIndex = (nextClipIndex + 1) % clips.Count;

        return clipToPlay;
    }

    // Resetăm indexul când pornim jocul (opțional)
    private void OnEnable() => nextClipIndex = 0;
}