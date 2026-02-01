using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public GameObject prefabToThrow; 
    public Transform throwPoint;    
    public float throwForce = 15f;
    public KeyCode throwKey = KeyCode.G; 

    void Update()
    {
        
        if (Time.frameCount % 60 == 0) { Debug.Log("Scriptul ruleaza..."); }
        if (Input.GetKeyDown(throwKey))
        {
            Debug.Log($"<color=cyan>[Throw] Tasta {throwKey} a fost detectata!</color>");

            // Verificare stare jucator
            if (PlayerStats.Instance != null && PlayerStats.Instance.currentHealth <= 0)
            {
                Debug.LogWarning("[Throw] Jucatorul este mort, aruncarea este blocata.");
                return;
            }

            TryThrow();
        }
    }

    void TryThrow()
    {
        // Verificare Singleton Inventory
        if (SimpleInventoryManager.Instance == null)
        {
            Debug.LogError("[Throw] SimpleInventoryManager.Instance este NULL! Ai pus scriptul pe un obiect in scena?");
            return;
        }

        Debug.Log($"[Throw] Inventar gasit. Potiuni disponibile: {SimpleInventoryManager.Instance.potionCount}");

        if (SimpleInventoryManager.Instance.potionCount > 0)
        {
            SimpleInventoryManager.Instance.potionCount--;
            ExecuteThrow();
        }
        else
        {
            Debug.LogWarning("[Throw] Nu ai destule potiuni (Count = 0).");
        }
    }

    void ExecuteThrow()
    {
        if (prefabToThrow == null) { Debug.LogError("Prefab lipsa!"); return; }

        // 1. Gasim camera principala
        Camera mainCam = Camera.main;
        if (mainCam == null) { Debug.LogError("Nu am gasit Camera.main!"); return; }

        // 2. Calculam pozitia de start: centrul camerei + un mic offset in fata
        // Asta previne coliziunea cu jucatorul
        Vector3 spawnPos = mainCam.transform.position + (mainCam.transform.forward * 0.8f); 
        
        // 3. Instantiem obiectul
        GameObject obj = Instantiate(prefabToThrow, spawnPos, mainCam.transform.rotation);
        
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 4. Forta merge DIRECT in fata camerei
            // Poti adauga si o mica forta in sus (up * 2f) daca vrei o traiectorie mai realista
            Vector3 throwDir = mainCam.transform.forward; 
            
            rb.AddForce(throwDir * throwForce, ForceMode.Impulse);
            
            // Rotatie aleatorie pentru realism
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }

        GameEvents.TriggerSound("Throw_Item");
    }
}