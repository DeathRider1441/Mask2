using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI Slots")]
    public Image[] slotIcons;      // Iconitele (copiii sloturilor)
    public TextMeshProUGUI stackText; // Textul de la Slot 1 (Eprubete)
    public RectTransform highlight;  // Imaginea de selectie (chenarul)
    public Transform[] slotTransforms; // Pozitiile sloturilor pentru highlight

    [Header("Date Inventar")]
    public int currentSlot = 0; // 0=Eprubete, 1=Masca1, 2=Masca2
    public int eprubeteCount = 0;

    void Awake() { Instance = this; }

    void Update()
    {
        // Selectie cu tastele 1, 2, 3
        if (Input.GetKeyDown(KeyCode.Alpha1)) { currentSlot = 0; UpdateUI(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { currentSlot = 1; UpdateUI(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { currentSlot = 2; UpdateUI(); }

        // Selectie cu rotita
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) { currentSlot = (currentSlot + 1) % 3; UpdateUI(); }
        else if (scroll < 0f) { currentSlot = (currentSlot - 1 + 3) % 3; UpdateUI(); }
    }

    void UpdateUI()
    {
        highlight.position = slotTransforms[currentSlot].position;
    }

    public void AddEprubeta(Sprite icon)
    {
        eprubeteCount++;
        slotIcons[0].sprite = icon;
        slotIcons[0].enabled = true;
        stackText.text = eprubeteCount.ToString();
    }
}