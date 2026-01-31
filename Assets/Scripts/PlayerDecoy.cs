using UnityEngine;

public class PlayerDecoy : MonoBehaviour
{
    [Header("Referinte")]
    public GameObject eprubetaPrefab; // Prefabul eprubetei care va fi aruncat
    public Transform throwPoint;      // Un punct gol in fata camerei
    public GameObject handVisual;     // Modelul eprubetei care se vede in mana

    [Header("Setari")]
    public float throwForce = 15f;
    public int eprubeteInStoc = 0;    // Cate ai in buzunar
    public int capacitateMaxima = 5;

    void Update()
    {
        // Daca ai cel putin o eprubeta, modelul din mana devine vizibil
        if (handVisual != null)
        {
            handVisual.SetActive(eprubeteInStoc > 0);
        }

        // Daca apesi click stanga si ai ce sa arunci
        if (Input.GetMouseButtonDown(0) && eprubeteInStoc > 0)
        {
            ThrowEprubeta();
        }
    }

    void ThrowEprubeta()
    {
        eprubeteInStoc--; // Scade numarul de eprubete (ex: de la 3 la 2)

        // Creeaza eprubeta fizica in lume
        GameObject proiectil = Instantiate(eprubetaPrefab, throwPoint.position, throwPoint.rotation);
        
        // Ii spunem eprubetei nou create ca este un proiectil, nu un obiect de colectat
        proiectil.GetComponent<EprubetaLogic>().SetAsProjectile();

        // Ii aplicam forta de aruncare
        Rigidbody rb = proiectil.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
        }
    }

    // Metoda folosita cand treci peste o eprubeta pe harta
    public bool ColecteazaEprubeta()
    {
        if (eprubeteInStoc < capacitateMaxima)
        {
            eprubeteInStoc++;
            Debug.Log("Eprubete in inventar: " + eprubeteInStoc);
            return true;
        }
        return false;
    }
}