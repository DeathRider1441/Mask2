using UnityEngine;
using UnityEngine.UI;

public class SimpleInventoryManager : MonoBehaviour
{
    // --- Logica de Singleton ---
    public static SimpleInventoryManager Instance { get; private set; }

    private void Awake()
    {
        // Ne asigurăm că există o singură instanță
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Opțional: Keep it across scenes
            // DontDestroyOnLoad(gameObject); 
        }
    }
    // ---------------------------

    [Header("Slot 1: Poțiuni")]
    public int potionCount = 0;
    public Image potionIcon;
    public Text potionText;

    [Header("Slot 2: Cheie")]
    public bool hasKey = false;
    public Image keyIcon;

    [Header("Slot 3: Mască")]
    public Image maskIcon; // Aceasta va fi mereu vizibilă

    void Start()
    {
        UpdateUI();
    }

    public void AddPotions(int amount)
    {
        potionCount += amount;
        UpdateUI();
    }

    public void UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            UpdateUI();
            Debug.Log("Poțiune folosită!");
        }
    }

    public void SetKey(bool state)
    {
        hasKey = state;
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Slot 1: Vizibil doar dacă ai poțiuni
        if (potionIcon != null) potionIcon.enabled = (potionCount > 0);
        if (potionText != null) potionText.text = potionCount > 0 ? "x" + potionCount : "";

        // Slot 2: Vizibil doar dacă ai cheia
        if (keyIcon != null) keyIcon.enabled = hasKey;

        // Slot 3: Masca este mereu activă conform cerinței tale
        if (maskIcon != null) maskIcon.enabled = true;
    }
}