using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class InventoryUIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement damageOverlay;
    
    // Referințe Sloturi (Containerele pătrate)
    private VisualElement potionSlot;
    private VisualElement keySlot; 
    
    // Referințe Texte
    private Label potionLabel;

    // Referințe Viață
    private VisualElement[] hearts = new VisualElement[3];
    private Coroutine damageFadeCoroutine;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        root = uiDocument.rootVisualElement;

        // 1. Luăm referința la SLOTURILE întregi (pătratul gri), nu doar la iconiță
        // Numele trebuie să fie exact ca în UXML
        potionSlot = root.Q<VisualElement>("slot-potions");
        keySlot = root.Q<VisualElement>("slot-key");
        
        potionLabel = root.Q<Label>("count-potions");
        damageOverlay = root.Q<VisualElement>("damage-overlay");

        for (int i = 0; i < 3; i++)
        {
            hearts[i] = root.Q<VisualElement>($"heart-{i + 1}");
        }

        GameEvents.OnPlayerHit += HandlePlayerHit;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerHit -= HandlePlayerHit;
    }

    private void HandlePlayerHit(int currentHealth)
    {
        UpdateHealthUI(currentHealth);
        TriggerDamageFlash();
    }

    private void UpdateHealthUI(int health)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            if (i < health)
            {
                hearts[i].style.opacity = 1f;
                hearts[i].style.backgroundColor = new StyleColor(new Color(1f, 0f, 0f, 0.8f));
            }
            else
            {
                hearts[i].style.opacity = 0.3f;
                hearts[i].style.backgroundColor = new StyleColor(Color.gray);
            }
        }
    }

    private void TriggerDamageFlash()
    {
        if (damageOverlay == null) return;
        if (damageFadeCoroutine != null) StopCoroutine(damageFadeCoroutine);
        damageFadeCoroutine = StartCoroutine(FadeDamageOverlay());
    }

    private IEnumerator FadeDamageOverlay()
    {
        float intensity = 0.5f;
        while (intensity > 0)
        {
            intensity -= Time.deltaTime * 1.5f;
            damageOverlay.style.backgroundColor = new StyleColor(new Color(1f, 0f, 0f, intensity));
            yield return null;
        }
        damageOverlay.style.backgroundColor = new StyleColor(new Color(1f, 0f, 0f, 0f));
    }

    void Update()
    {
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        if (SimpleInventoryManager.Instance == null) return;

        // --- POTIUNI ---
        if (potionSlot != null)
        {
            bool hasPotions = SimpleInventoryManager.Instance.potionCount > 0;
            
            // Folosim VISIBILITY: Slotul rămâne acolo, dar devine invizibil
            // Astfel nu se decalează celelalte elemente
            potionSlot.style.visibility = hasPotions ? Visibility.Visible : Visibility.Hidden;

            if (potionLabel != null)
                potionLabel.text = "x" + SimpleInventoryManager.Instance.potionCount;
        }

        // --- CHEIE ---
        if (keySlot != null)
        {
            bool hasKey = SimpleInventoryManager.Instance.hasKey;

            // FIXUL: Folosim Visibility în loc de Display
            // Visibility.Hidden = E invizibil, dar Ocupă spațiu (Slotul 3 nu sare în locul lui)
            // Visibility.Visible = Se vede normal
            keySlot.style.visibility = hasKey ? Visibility.Visible : Visibility.Hidden;
            
            // Alternativă: Dacă vrei să vezi slotul gol (gri) și doar iconița să apară/dispară,
            // trebuie să cauți copilul "icon-key" și să îi schimbi lui opacity.
            // Dar momentan, codul acesta repară "săritul" sloturilor.
        }
    }
}