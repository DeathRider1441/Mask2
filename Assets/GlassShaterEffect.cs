using UnityEngine;

public class GlassShatterController : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] private ParticleSystem shatterEffect;
    
    [Header("Settings")]
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool destroyAfterPlay = true;
    [SerializeField] private float destroyDelay = 2f;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip glassBreakSound;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.8f;
    
    private void Start()
    {
        // Găsim ParticleSystem dacă nu e setat
        if (shatterEffect == null)
        {
            shatterEffect = GetComponent<ParticleSystem>();
        }
        
        // Găsim AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Play on start dacă e activat
        if (playOnStart)
        {
            PlayShatter();
        }
    }
    
    public void PlayShatter()
    {
        if (shatterEffect != null)
        {
            shatterEffect.Play();
        }
        
        // Redăm sunetul
        if (audioSource != null && glassBreakSound != null)
        {
            audioSource.PlayOneShot(glassBreakSound, volume);
        }
        
        // Distrugem obiectul după ce efectul se termină
        if (destroyAfterPlay)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
    
    public void PlayShatterAt(Vector3 position)
    {
        transform.position = position;
        PlayShatter();
    }
    
    public void PlayShatterAt(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        PlayShatter();
    }
}
