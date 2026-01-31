using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaminaSystem : MonoBehaviour
{
    // Singleton Instance
    public static StaminaSystem Instance { get; private set; }
    
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;
    [SerializeField] private float lowStaminaThreshold = 20f; // Pragul pentru low stamina
    
    [Header("Regeneration Settings")]
    [SerializeField] private float staminaIncreaseRate = 15f; // Cât de repede crește când e activat
    [SerializeField] private float staminaDecreaseRate = 10f; // Cât de repede scade când e dezactivat
    [SerializeField] private float delayAfterLowStamina = 3f; // Delay de 3 secunde
    
    [Header("UI Settings")]
    [SerializeField] private Slider staminaSlider; // Optional - pentru UI
    [SerializeField] private Image staminaFillImage; // Optional - pentru UI
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.yellow;
    [SerializeField] private Color lowStaminaColor = Color.red; // Culoare pentru stamina joasă
    [SerializeField] private Color cooldownColor = Color.cyan; // Culoare în timpul cooldown-ului
    
    [Header("Status")]
    [SerializeField] private bool isStaminaActive = false;
    [SerializeField] private bool isOnCooldown = false; // Flag pentru cooldown
    [SerializeField] private bool wasLowStamina = false; // Flag pentru a ști dacă a fost sub 20
    [SerializeField] private float cooldownTimer = 0f; // Timer pentru cooldown
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Opțional: păstrează obiectul între scene
        // DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        // Inițializăm stamina la valoarea maximă
        currentStamina = maxStamina;
        
        // Setăm UI-ul dacă există
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
        
        UpdateStaminaUI();
    }
    
    private void Update()
    {
        // Detectăm apăsarea tastei Shift
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            ToggleStamina();
        }
        
        // Procesăm cooldown-ul
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                wasLowStamina = false;
                
                // AUTOMAT activăm stamina după cooldown
                isStaminaActive = true;
                
                Debug.Log("Cooldown terminat! Stamina pornește AUTOMAT!");
            }
        }
        
        // Procesăm stamina în funcție de stare
        ProcessStamina();
        
        // Actualizăm UI-ul
        UpdateStaminaUI();
    }
    
    private void ToggleStamina()
    {
        // Dacă suntem deja în cooldown, ignorăm apăsarea
        if (isOnCooldown)
        {
            Debug.Log("În cooldown... Așteptați!");
            return;
        }
        
        // Verificăm dacă stamina e sub pragul minim când vrem să activăm
        if (!isStaminaActive && currentStamina < lowStaminaThreshold)
        {
            Debug.Log("Stamina prea joasă! Se oprește și va porni automat în " + delayAfterLowStamina + " secunde...");
            
            // Activăm cooldown-ul
            isOnCooldown = true;
            wasLowStamina = true;
            cooldownTimer = delayAfterLowStamina;
            
            // IMPORTANT: Stamina rămâne DEZACTIVATĂ (nu crește, nu scade)
            isStaminaActive = false;
            
            return;
        }
        
        // Toggle normal când stamina e peste 20
        isStaminaActive = !isStaminaActive;
        
        Debug.Log("Stamina " + (isStaminaActive ? "ACTIVATĂ" : "DEZACTIVATĂ"));
    }
    
    private void ProcessStamina()
    {
        // Dacă suntem în cooldown, stamina NU se modifică deloc
        if (isOnCooldown)
        {
            // Stamina e înghețată, nu face nimic
            return;
        }
        
        if (isStaminaActive)
        {
            // Când e activă, stamina CREȘTE
            currentStamina += staminaIncreaseRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
        else
        {
            // Când e dezactivă, stamina SCADE
            currentStamina -= staminaDecreaseRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }
    
    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
        
        if (staminaFillImage != null)
        {
            // Schimbăm culoarea în funcție de stare
            if (isOnCooldown)
            {
                staminaFillImage.color = cooldownColor; // Cyan în timpul cooldown-ului
            }
            else if (currentStamina < lowStaminaThreshold)
            {
                staminaFillImage.color = lowStaminaColor; // Roșu când e joasă
            }
            else
            {
                staminaFillImage.color = isStaminaActive ? activeColor : inactiveColor;
            }
        }
    }
    
    // Funcții publice pentru alte scripturi
    public float GetCurrentStamina()
    {
        return currentStamina;
    }
    
    public float GetStaminaPercentage()
    {
        return (currentStamina / maxStamina) * 100f;
    }
    
    public bool IsStaminaActive()
    {
        return isStaminaActive;
    }
    
    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
    
    public float GetCooldownTimeRemaining()
    {
        return isOnCooldown ? cooldownTimer : 0f;
    }
    
    public void SetStaminaActive(bool active)
    {
        // Dacă suntem în cooldown, nu permitem schimbări manuale
        if (isOnCooldown)
        {
            return;
        }
        
        // Verificăm aceleași condiții ca și la toggle
        if (active && currentStamina < lowStaminaThreshold && !isStaminaActive)
        {
            isOnCooldown = true;
            wasLowStamina = true;
            cooldownTimer = delayAfterLowStamina;
            isStaminaActive = false;
            return;
        }
        
        isStaminaActive = active;
    }
    
    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }
    
    public void RemoveStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }
    
    public void ResetStamina()
    {
        currentStamina = maxStamina;
        isOnCooldown = false;
        wasLowStamina = false;
        cooldownTimer = 0f;
    }
    
    public void ResetCooldown()
    {
        isOnCooldown = false;
        wasLowStamina = false;
        cooldownTimer = 0f;
    }
}