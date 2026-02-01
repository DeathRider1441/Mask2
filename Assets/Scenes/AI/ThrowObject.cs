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
        if (prefabToThrow == null)
        {
            Debug.LogError("[Throw] PrefabToThrow nu este asignat in Inspector!");
            return;
        }
        if (throwPoint == null)
        {
            Debug.LogError("[Throw] ThrowPoint nu este asignat in Inspector!");
            return;
        }

        GameObject obj = Instantiate(prefabToThrow, throwPoint.position, throwPoint.rotation);
        Debug.Log($"[Throw] Obiect instantiat: {obj.name}");
        
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            Debug.Log("[Throw] Forta fizica a fost aplicata.");
        }
        else
        {
            Debug.LogError($"[Throw] Prefab-ul {obj.name} NU are o componenta Rigidbody!");
        }

        GameEvents.TriggerSound("Throw_Item");
    }
}