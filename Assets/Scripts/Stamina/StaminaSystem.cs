using UnityEngine;
using UnityEngine.UI;

public class MaskSystem : MonoBehaviour
{
    public static MaskSystem Instance { get; private set; }
    
    [Header("Mask Settings")]
    [SerializeField] private float maxCharge = 100f;
    [SerializeField] private float currentCharge;
    [SerializeField] private float autoFailureThreshold = 5f; // Dacă scade sub 5, masca cade singură
    
    [Header("Rates")]
    [SerializeField] private float consumptionRate = 20f; // Cât de repede se consumă (secunde)
    [SerializeField] private float rechargeRate = 15f;    // Cât de repede se încarcă când nu e folosită
    [SerializeField] private float cooldownDuration = 3f; // Cooldown forțat dacă se golește
    
    [Header("UI (Optional)")]
    [SerializeField] private Slider maskSlider;
    [SerializeField] private Image maskFillImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color cooldownColor = Color.red;

    [Header("Status")]
    [SerializeField] private bool isMaskOn = false;
    [SerializeField] private bool isOnCooldown = false;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        currentCharge = maxCharge;
        if (maskSlider != null) maskSlider.maxValue = maxCharge;
    }

    private void Update()
    {
        // 1. Input - Tasta Shift pune/scoate masca
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleMask();
        }

        // 2. Gestionare Cooldown
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                isOnCooldown = false;
                Debug.Log("Masca poate fi folosită din nou!");
            }
        }

        // 3. Procesare Consum / Reîncărcare
        HandleLogic();

        // 4. Update Vizual
        UpdateUI();
    }

    private void ToggleMask()
    {
        if (isOnCooldown) return;

        // Nu poți pune masca dacă e aproape goală
        if (!isMaskOn && currentCharge < autoFailureThreshold) return;

        isMaskOn = !isMaskOn;
    }

    private void HandleLogic()
    {
        if (isMaskOn)
        {
            // Consumăm masca
            currentCharge -= consumptionRate * Time.deltaTime;

            // Dacă s-a golit, o scoatem forțat și intrăm în cooldown
            if (currentCharge <= 0)
            {
                currentCharge = 0;
                ForceRemoveMask();
            }
        }
        else
        {
            // Se reîncarcă doar când NU este pe față
            currentCharge += rechargeRate * Time.deltaTime;
        }

        currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
    }

    private void ForceRemoveMask()
    {
        isMaskOn = false;
        isOnCooldown = true;
        cooldownTimer = cooldownDuration;
        Debug.Log("Masca s-a descărcat! Cooldown activat.");
    }

    public bool IsPlayerGhosted()
    {
        // Ești invizibil DOAR dacă masca e pe față și NU ești în cooldown
        return isMaskOn && !isOnCooldown;
    }

    private void UpdateUI()
    {
        if (maskSlider != null) maskSlider.value = currentCharge;
        if (maskFillImage != null)
        {
            maskFillImage.color = isOnCooldown ? cooldownColor : normalColor;
        }
    }
}