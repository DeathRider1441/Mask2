using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class InventoryUIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement damageOverlay;
    
    // Elemente Inventar
    private VisualElement potionIcon;
    private VisualElement keyIcon;
    private Label potionLabel;

    // Elemente Viață
    private VisualElement[] hearts = new VisualElement[3];
    private Coroutine damageFadeCoroutine;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        root = uiDocument.rootVisualElement;

        // Referințe Inventar
        potionIcon = root.Q<VisualElement>("icon-potions");
        keyIcon = root.Q<VisualElement>("icon-key");
        potionLabel = root.Q<Label>("count-potions");

        // Referință Overlay Damage
        damageOverlay = root.Q<VisualElement>("damage-overlay");

        // Referințe Viață
        for (int i = 0; i < 3; i++)
        {
            hearts[i] = root.Q<VisualElement>($"heart-{i + 1}");
        }

        // ABONARE LA EVENIMENTE
        GameEvents.OnPlayerHit += HandlePlayerHit;
    }

    void OnDisable()
    {
        // DEZABONARE
        GameEvents.OnPlayerHit -= HandlePlayerHit;
    }

    // Această metodă rulează DOAR când jucătorul ia damage
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
                hearts[i].style.scale = new StyleScale(new Scale(Vector2.one));
            }
            else
            {
                hearts[i].style.opacity = 0.3f;
                hearts[i].style.backgroundColor = new StyleColor(Color.gray);
                hearts[i].style.scale = new StyleScale(new Scale(new Vector3(0.8f, 0.8f, 1f)));
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
        float intensity = 0.5f; // Cât de roșu să fie ecranul la început
        
        while (intensity > 0)
        {
            intensity -= Time.deltaTime * 1.5f; // Viteza cu care dispare roșul
            damageOverlay.style.backgroundColor = new StyleColor(new Color(1f, 0f, 0f, intensity));
            yield return null;
        }

        damageOverlay.style.backgroundColor = new StyleColor(new Color(1f, 0f, 0f, 0f));
    }

    // Inventarul îl lăsăm momentan în Update dacă nu ai făcut semnal și pentru el
    void Update()
    {
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        if (SimpleInventoryManager.Instance != null && potionIcon != null)
        {
            bool hasPotions = SimpleInventoryManager.Instance.potionCount > 0;
            potionIcon.style.display = hasPotions ? DisplayStyle.Flex : DisplayStyle.None;
            
            if (potionLabel != null)
            {
                potionLabel.text = "x" + SimpleInventoryManager.Instance.potionCount;
                potionLabel.style.display = hasPotions ? DisplayStyle.Flex : DisplayStyle.None;
            }

            keyIcon.style.display = SimpleInventoryManager.Instance.hasKey ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}