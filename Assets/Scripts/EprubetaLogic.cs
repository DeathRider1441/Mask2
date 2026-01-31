using UnityEngine;

public class EprubetaLogic : MonoBehaviour
{
    public float razaZgomot = 12f;
    private bool esteAruncata = false;

    // Aceasta functie schimba eprubeta din "obiect de luat" in "obiect care face zgomot"
    public void SetAsProjectile()
    {
        esteAruncata = true;
        if (GetComponent<Collider>()) GetComponent<Collider>().isTrigger = false;
    }

    // Cand jucatorul atinge eprubeta de pe masa
    private void OnTriggerEnter(Collider other)
    {
        if (!esteAruncata && other.CompareTag("Player"))
        {
            PlayerDecoy player = other.GetComponent<PlayerDecoy>();
            if (player != null && player.ColecteazaEprubeta())
            {
                Destroy(gameObject); // Dispare de pe masa si intra in inventar
            }
        }
    }

    // Cand eprubeta aruncata loveste podeaua
    private void OnCollisionEnter(Collision collision)
    {
        if (esteAruncata)
        {
            AlertaInamici();
            // Poti pune aici un efect de particule sau sunet de sticla sparta
            Destroy(gameObject, 0.5f); 
        }
    }

    void AlertaInamici()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, razaZgomot);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                // Trimite inamicul la locul impactului
                hit.GetComponent<EprubetaEnemy>().InvestigaZgomot(transform.position);
            }
        }
    }
}