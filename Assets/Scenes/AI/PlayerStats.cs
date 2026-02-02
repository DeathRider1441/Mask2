using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    public int currentHealth;
    
    // Flag-ul pentru a verifica dacă jucătorul este deja mort
    private bool isDead = false;

    [Header("Death Animation")]
    [SerializeField] private float fallDuration = 1.5f;
    [SerializeField] private float tiltAngle = 75f; 
    [SerializeField] private float sinkAmount = 1.3f;

    [Header("Sound Keys")]
    [SerializeField] private string damageSoundName = "PlayerDamage";
    [SerializeField] private string deathSoundName = "PlayerDeath";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ResetStats();
    }

    public void ResetStats()
    {
        isDead = false; // Resetăm starea la restart
        currentHealth = maxHealth;
        GameEvents.TriggerPlayerHit(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        // Dacă jucătorul este deja mort, ignorăm orice damage ulterior
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            GameEvents.TriggerPlayerHit(currentHealth);
            GameEvents.TriggerSound(damageSoundName);
            Die();
        }
        else
        {
            // Declanșăm sunetul și UI-ul doar dacă mai are viață
            GameEvents.TriggerSound(damageSoundName);
            GameEvents.TriggerPlayerHit(currentHealth);
        }
    }

    private void Die()
    {
        if (isDead) return; // Verificare redundantă de siguranță
        isDead = true;

        Debug.Log("<color=red>Jucătorul a murit!</color>");

        GameEvents.TriggerSound(deathSoundName);
        GameEvents.TriggerPlayerDeath();

        // 1. Dezactivăm controlul jucătorului
        if (TryGetComponent<FirstPersonController>(out var fpc))
        {
            fpc.playerCanMove = false;
            fpc.cameraCanMove = false;
            fpc.enableHeadBob = false;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 2. Pornim animația de cădere a camerei
        StartCoroutine(DeathAnimationRoutine());
    }

    private IEnumerator DeathAnimationRoutine()
    {
        Camera cam = GetComponentInChildren<Camera>();
        if (cam == null) yield break;

        Vector3 startPos = cam.transform.localPosition;
        Quaternion startRot = cam.transform.localRotation;

        Vector3 endPos = new Vector3(startPos.x, startPos.y - sinkAmount, startPos.z);
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, tiltAngle);

        float elapsed = 0;
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;

            // Curbă tip "SmoothStep"
            float curve = t * t * (3f - 2f * t);

            cam.transform.localPosition = Vector3.Lerp(startPos, endPos, curve);
            cam.transform.localRotation = Quaternion.Slerp(startRot, endRot, curve);

            yield return null;
        }
    }
}