using UnityEngine;
using UnityEngine.AI;

public class EprubetaEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 pozitieInitiala;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pozitieInitiala = transform.position;
    }

    public void InvestigaZgomot(Vector3 punctZgomot)
    {
        agent.SetDestination(punctZgomot);
        
        // Dupa 5 secunde se intoarce la locul lui
        CancelInvoke();
        Invoke("Intoarcere", 5f);
    }

    void Intoarcere()
    {
        agent.SetDestination(pozitieInitiala);
    }
}