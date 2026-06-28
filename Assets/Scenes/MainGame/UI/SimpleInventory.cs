using UnityEngine;
using UnityEngine.UI;

public class SimpleInventoryManager : MonoBehaviour
{
    // --- Logica de Singleton ---
    public static SimpleInventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
    }
    // ---------------------------

    [Header("Slot 1: Potiuni")]
    public int potionCount = 0;
    public Image potionIcon;          // poate ramane gol -> il cream singuri din sprite
    public Text potionText;

    [Header("Slot 1: Iconita automata (NOU)")]
    [Tooltip("Trage AICI direct PNG-ul potion_icon. Daca 'Potion Icon' e gol, " +
             "scriptul creeaza singur iconita in acelasi slot ca textul x2.")]
    public Sprite potionSprite;

    [Header("Slot 2: Cheie")]
    public bool hasKey = false;
    public Image keyIcon;

    [Header("Slot 3: Masca")]
    public Image maskIcon; // mereu vizibila

    void Start()
    {
        AutoCreatePotionIcon();
        UpdateUI();
    }

    // Daca nu ai legat un Image la potionIcon dar ai dat un sprite + ai potionText,
    // construim iconita ca si copil al slotului in care e deja textul "x2".
    private void AutoCreatePotionIcon()
    {
        if (potionIcon != null) return;                 // ai deja un Image legat -> nu facem nimic
        if (potionSprite == null || potionText == null) return;

        Transform slot = potionText.transform.parent != null
                         ? potionText.transform.parent
                         : potionText.transform;

        var go = new GameObject("PotionIcon_Auto", typeof(RectTransform));
        go.transform.SetParent(slot, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(8f, 8f);             // mic padding fata de marginea slotului
        rt.offsetMax = new Vector2(-8f, -8f);
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        var img = go.AddComponent<Image>();
        img.sprite = potionSprite;
        img.preserveAspect = true;
        img.raycastTarget = false;

        // il punem chiar inaintea textului "x2" -> iconita sub text, peste fundalul slotului
        go.transform.SetSiblingIndex(potionText.transform.GetSiblingIndex());

        potionIcon = img;
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
            Debug.Log("Potiune folosita!");
        }
    }

    public void SetKey(bool state)
    {
        hasKey = state;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (potionIcon != null) potionIcon.enabled = (potionCount > 0);
        if (potionText != null) potionText.text = potionCount > 0 ? "x" + potionCount : "";

        if (keyIcon != null) keyIcon.enabled = hasKey;

        if (maskIcon != null) maskIcon.enabled = true;
    }
}