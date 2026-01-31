using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public GameObject prefabToThrow; // Obiectul pe care îl arunci (ex: Piatră)
    public Transform throwPoint;    // Locul de unde pleacă obiectul (ex: poziția camerei)
    public float throwForce = 15f;
    public KeyCode throwKey = KeyCode.G; // Tasta G pentru aruncare

    void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            Throw();
        }
    }

    void Throw()
    {
        // Creăm obiectul
        GameObject obj = Instantiate(prefabToThrow, throwPoint.position, throwPoint.rotation);
        
        // Luăm Rigidbody-ul ca să îi dăm viteză
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Aruncăm în direcția în care privește camera
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
        }
    }
}