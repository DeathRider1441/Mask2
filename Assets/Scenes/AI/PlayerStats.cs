using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Instanța Singleton
    public static PlayerStats Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    public int currentHealth;

    private void Awake()
    {
        // Logica de Singleton: asigurăm că există o singură instanță
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // Dacă vrei ca viața să nu se reseteze la schimbarea scenei:
        // DontDestroyOnLoad(gameObject);

        ResetStats();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Jucătorul a luat damage! Viață rămasă: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ResetStats()
    {
        currentHealth = maxHealth;
    }

    private void Die()
    {
        Debug.Log("Jucătorul a murit!");
        // Aici poți declanșa un ecran de Game Over sau reîncărcarea scenei
    }
}