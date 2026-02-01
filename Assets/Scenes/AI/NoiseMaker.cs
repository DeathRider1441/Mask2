using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    public string soundEventName = "Impact_Noise"; // Numele din SoundManager
    public float noiseRadius = 15f;                // Cât de departe aud inamicii
    private bool hasHit = false;                   // Să nu facă zgomot de mai multe ori la rând

    private void OnCollisionEnter(Collision collision)
    {
        // Verificăm dacă a lovit ceva solid (nu jucătorul sau un trigger)
        if (!hasHit)
        {
            // 1. Trimitem sunetul către SoundManager pentru auzul jucătorului
            GameEvents.TriggerSound(soundEventName);

            // 2. Trimitem semnalul de zgomot către AI
            GameEvents.TriggerNoise(transform.position, noiseRadius);

            hasHit = true;

            // Opțional: Distrugem obiectul după câteva secunde ca să nu aglomerăm scena
            Destroy(gameObject, 1f);
        }
    }
}