using UnityEngine;

// Se pune pe GameObject-ul "Story 6" (ultima hartie din scena).
// Ascunde cheia la inceput si o face sa apara cand jucatorul deschide Story 6 cu "Q".
public class RevealKeyOnRead : MonoBehaviour
{
    [Header("Referinte")]
    [Tooltip("Cheia care e deja pusa in scena (instanta lui Key_A). Trage-o aici din Hierarchy.")]
    [SerializeField] private GameObject keyObject;

    [Header("Interactiune incorporata")]
    [Tooltip("Daca e bifat, scriptul singur asculta tasta cand jucatorul e in raza (trigger). " +
             "Debifeaza daca preferi sa chemi RevealKey() din scriptul tau de hartii.")]
    [SerializeField] private bool useBuiltInInteraction = true;
    [SerializeField] private KeyCode readKey = KeyCode.Q;
    [SerializeField] private string playerTag = "Player";

    private bool playerInRange = false;
    private bool keyRevealed = false;

    void Start()
    {
        // Cheia incepe ascunsa (las-o ACTIVA in editor; scriptul o ascunde el).
        if (keyObject != null)
            keyObject.SetActive(false);
        else
            Debug.LogWarning("RevealKeyOnRead: nu ai asignat 'Key Object' (cheia Key_A din scena).");
    }

    void Update()
    {
        if (!useBuiltInInteraction || keyRevealed || !playerInRange) return;

        if (Input.GetKeyDown(readKey))
            RevealKey();
    }

    // Poate fi chemata si din afara (din scriptul tau de hartii, cand se deschide Story 6).
    public void RevealKey()
    {
        if (keyRevealed) return;
        keyRevealed = true;

        if (keyObject != null)
            keyObject.SetActive(true);

        Debug.Log("Story 6 citit -> cheia a aparut in scena.");
    }

    // Detecteaza jucatorul in raza (necesita un Collider cu Is Trigger pe Story 6).
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag)) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag)) playerInRange = false;
    }
}