using UnityEngine;

public class SimpleEnemy : NPCBase
{
    [Header("Simple Enemy Settings")]
    [Tooltip("Dacă e bifat, inamicul va scoate un sunet când ajunge la fiecare punct.")]
    public bool playSoundOnPointReached = false;

    protected override void Start()
    {
        // Apelează Start din NPCBase, care la rândul lui apelează Start din Entity
        base.Start();
        
        Debug.Log($"{gameObject.name} a fost inițializat cu {currentHealth} HP.");
    }

    // Aici poți face override la metode dacă vrei comportament specific
    public override void ToPatrol()
    {
        base.ToPatrol();
        // Exemplu: Poți schimba viteza când patrulează
        // SetSpeed(3f); 
    }

    protected override void Die()
    {
        // Poți adăuga efecte specifice înainte de distrugerea obiectului
        Debug.Log($"Inamicul {gameObject.name} a fost eliminat silențios.");
        
        // Nu uita să apelezi baza pentru a activa sistemul de Loot și distrugere
        base.Die();
    }

    // --- VIZUALIZARE TRASEU ÎN EDITOR ---
    private void OnDrawGizmos()
    {
        if (patrolRoute == null || patrolRoute.Count == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolRoute.Count; i++)
        {
            if (patrolRoute[i].point == null) continue;

            // Desenăm o sferă la fiecare punct
            Gizmos.DrawWireSphere(patrolRoute[i].point.position, 0.5f);

            // Desenăm o linie către următorul punct (pentru a vedea traseul)
            if (i < patrolRoute.Count - 1 && patrolRoute[i + 1].point != null)
            {
                Gizmos.DrawLine(patrolRoute[i].point.position, patrolRoute[i + 1].point.position);
            }
        }
    }
}