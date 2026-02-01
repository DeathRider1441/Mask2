using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float interactionDistance = 3.5f;
    [SerializeField] private LayerMask interactableLayer;

    private VisualElement interactPrompt;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        
        var uiDoc = Object.FindFirstObjectByType<UIDocument>();
        if (uiDoc != null)
        {
            interactPrompt = uiDoc.rootVisualElement.Q<VisualElement>("interact-prompt");
            if (interactPrompt == null) 
                Debug.LogError("UI Error: Nu am găsit elementul cu numele 'interact-prompt' în UXML!");
        }
    }

    void Update()
    {
        if (PlayerStats.Instance != null && PlayerStats.Instance.currentHealth <= 0)
        {
            ShowPrompt(false);
            return;
        }

        if (playerCamera != null) CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.cyan);

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            // LOG 1: Vedem ce obiect lovim exact
            // Debug.Log($"Raycast a lovit: {hit.collider.name} pe Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // LOG 2: Am găsit ceva ce poate fi apăsat
                // Debug.Log("<color=green>Interactabil detectat!</color>");
                ShowPrompt(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Tasta E apăsată pe: " + hit.collider.name);
                    interactable.Interact();
                }
                return; 
            }
        }

        ShowPrompt(false);
    }

    private void ShowPrompt(bool visible)
    {
        if (interactPrompt == null) return;

        // Dacă starea se schimbă, dăm un log scurt
        if (visible && interactPrompt.style.display == DisplayStyle.None)
            Debug.Log("UI: Afișez prompt-ul E");

        interactPrompt.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}