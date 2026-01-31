using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour
{
    public static PlayerHealthSystem Instance { get; private set; }
    
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    
    [Header("UI References")]
    [SerializeField] private Image[] healthImages; // Array cu cele 3 imagini
    
    [Header("Visual Settings")]
    [SerializeField] private Color fullHealthColor = Color.red; // Culoare când are viață
    [SerializeField] private Color emptyHealthColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Culoare când e pierdută (gri transparent)
    [SerializeField] private bool useScaleEffect = true; // Micșorează când pierde viață
    [SerializeField] private float emptyScale = 0.8f; // Cât de mic devine când e gol
    [SerializeField] private float scaleSpeed = 5f; // Viteza de scale
    
    [Header("Animation Settings")]
    [SerializeField] private bool usePulseOnDamage = true; // Pulsează când primește damage
    [SerializeField] private float pulseScale = 1.3f; // Cât de mare devine în puls
    [SerializeField] private float pulseDuration = 0.2f; // Durata pulsului
    
    private Vector3[] originalScales; // Scalele originale ale cercurilor
    private bool[] isPulsing; // Track dacă fiecare cerc pulsează
    
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        // Inițializăm health-ul
        currentHealth = maxHealth;
        
        // Salvăm scalele originale
        originalScales = new Vector3[healthImages.Length];
        isPulsing = new bool[healthImages.Length];
        
        for (int i = 0; i < healthImages.Length; i++)
        {
            if (healthImages[i] != null)
            {
                originalScales[i] = healthImages[i].transform.localScale;
                isPulsing[i] = false;
            }
        }
        
        // Update UI
        UpdateHealthUI();
    }
    
    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthImages.Length; i++)
        {
            if (healthImages[i] == null) continue;
            
            // Verificăm dacă această viață e activă
            bool isActive = i < currentHealth;
            
            // Setăm culoarea
            healthImages[i].color = isActive ? fullHealthColor : emptyHealthColor;
            
            // Setăm scala dacă e activat efectul
            if (useScaleEffect && !isPulsing[i])
            {
                float targetScale = isActive ? 1f : emptyScale;
                healthImages[i].transform.localScale = originalScales[i] * targetScale;
            }
        }
    }
    
    // Funcții publice
    
    public void TakeDamage(int amount = 1)
    {
        if (currentHealth <= 0) return;
        
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log("Player took damage! Current health: " + currentHealth);
        
        // Pulsăm cercul care s-a pierdut
        if (usePulseOnDamage && currentHealth < maxHealth)
        {
            int lostHeartIndex = currentHealth; // Index-ul inimii pierdute
            if (lostHeartIndex >= 0 && lostHeartIndex < healthImages.Length)
            {
                StartCoroutine(PulseHeart(lostHeartIndex));
            }
        }
        
        UpdateHealthUI();
        
        // Verificăm dacă playerul a murit
        if (currentHealth <= 0)
        {
            OnPlayerDeath();
        }
    }
    
    public void Heal(int amount = 1)
    {
        if (currentHealth >= maxHealth) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log("Player healed! Current health: " + currentHealth);
        
        // Pulsăm cercul care s-a recuperat
        if (usePulseOnDamage)
        {
            int healedHeartIndex = currentHealth - 1; // Index-ul inimii recuperate
            if (healedHeartIndex >= 0 && healedHeartIndex < healthImages.Length)
            {
                StartCoroutine(PulseHeart(healedHeartIndex));
            }
        }
        
        UpdateHealthUI();
    }
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        Debug.Log("Player health reset!");
    }
    
    public void SetMaxHealth(int newMax)
    {
        maxHealth = newMax;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
    
    private void OnPlayerDeath()
    {
        Debug.Log("PLAYER DIED!");
        
        // Aici poți adăuga logica pentru moartea playerului
        // Ex: restart level, game over screen, etc.
        
        // Exemplu: Resetează după 2 secunde
        Invoke("ResetHealth", 2f);
    }
    
    private System.Collections.IEnumerator PulseHeart(int index)
    {
        if (index < 0 || index >= healthImages.Length || healthImages[index] == null)
            yield break;
        
        isPulsing[index] = true;
        
        Transform heartTransform = healthImages[index].transform;
        Vector3 originalScale = originalScales[index];
        Vector3 targetScale = originalScale * pulseScale;
        
        // Scale up
        float elapsed = 0f;
        while (elapsed < pulseDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (pulseDuration / 2f);
            heartTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Scale down
        elapsed = 0f;
        bool isActive = index < currentHealth;
        float finalScale = (useScaleEffect && !isActive) ? emptyScale : 1f;
        Vector3 finalScaleVector = originalScale * finalScale;
        
        while (elapsed < pulseDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (pulseDuration / 2f);
            heartTransform.localScale = Vector3.Lerp(targetScale, finalScaleVector, t);
            yield return null;
        }
        
        heartTransform.localScale = finalScaleVector;
        isPulsing[index] = false;
    }
    
    // Getters
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}