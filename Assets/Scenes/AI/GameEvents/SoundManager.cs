using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public List<SoundEvent> soundDatabase;
    private AudioSource audioSource; // Pentru sunete de tip explozii, UI, etc.
    private Dictionary<string, SoundEvent> soundDictionary;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        soundDictionary = new Dictionary<string, SoundEvent>();

        foreach (var ev in soundDatabase)
        {
            if (ev != null && !soundDictionary.ContainsKey(ev.eventName))
                soundDictionary.Add(ev.eventName, ev);
        }
    }

    void OnEnable() 
    {
        GameEvents.OnPlaySound += PlayByName;
        GameEvents.OnStopSound += StopByName; 
    }

    void OnDisable() 
    {
        GameEvents.OnPlaySound -= PlayByName;
        GameEvents.OnStopSound -= StopByName;
    }

    private void PlayByName(string name)
    {
        if (soundDictionary.TryGetValue(name, out SoundEvent ev))
        {
            AudioClip clipToPlay = ev.GetNextClip();
            if (clipToPlay == null) return;

            audioSource.pitch = ev.pitch;
            audioSource.PlayOneShot(clipToPlay, ev.volume);
        }
    }

    private void StopByName(string name)
    {
        // Pentru pași, cea mai simplă metodă de "Cut" este să oprim AudioSource-ul.
        // Dacă muzica este pe un ALT obiect cu un ALT AudioSource, ea NU se va opri.
        if (soundDictionary.ContainsKey(name))
        {
            audioSource.Stop(); 
        }
    }
}