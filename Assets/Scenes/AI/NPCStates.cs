using UnityEngine;
using UnityEngine.AI;

// ----------------------------------------------------------------------
// 1. PATROL STATE (Logica Ping-Pong 1-2-3-2-1)
// ----------------------------------------------------------------------
public class PatrolState : INPCState
{
    public NPCBase.NPCStateID StateID => NPCBase.NPCStateID.Patrol;

    public void EnterState(NPCBase npc)
    {
        if (npc.Agent.isOnNavMesh) npc.Agent.isStopped = false;
        
        // Verificăm dacă avem unde să mergem
        if (npc.patrolRoute.Count == 0)
        {
            npc.ToIdle();
            return;
        }

        MoveToCurrentPoint(npc);
    }

    public void DoState(NPCBase npc)
    {
        // Verificăm dacă am ajuns la punctul curent
        if (!npc.Agent.pathPending && npc.Agent.remainingDistance <= npc.Agent.stoppingDistance)
        {
            // Luăm timpul de așteptare configurat pentru punctul la care tocmai am ajuns
            float timeToWait = npc.patrolRoute[npc.currentPatrolIndex].waitTime;
            
            // Calculăm indexul pentru URMĂTORUL punct (pregătim viitorul)
            UpdatePatrolIndex(npc);
            
            // Trecem în starea de așteptare
            npc.ToWait(timeToWait);
        }
    }

    private void MoveToCurrentPoint(NPCBase npc)
    {
        Transform targetTransform = npc.patrolRoute[npc.currentPatrolIndex].point;
        if (targetTransform != null)
        {
            npc.Agent.SetDestination(targetTransform.position);
        }
    }

    private void UpdatePatrolIndex(NPCBase npc)
    {
        if (npc.patrolRoute.Count <= 1) return;

        if (npc.isForward)
        {
            npc.currentPatrolIndex++;
            // Dacă am depășit capătul listei, ne întoarcem
            if (npc.currentPatrolIndex >= npc.patrolRoute.Count)
            {
                npc.currentPatrolIndex = npc.patrolRoute.Count - 2; // Punctul penultim
                npc.isForward = false;
            }
        }
        else
        {
            npc.currentPatrolIndex--;
            // Dacă am ajuns sub începutul listei, mergem iar înainte
            if (npc.currentPatrolIndex < 0)
            {
                npc.currentPatrolIndex = 1; // Al doilea punct
                npc.isForward = true;
            }
        }

        // Siguranță împotriva erorilor de index
        npc.currentPatrolIndex = Mathf.Clamp(npc.currentPatrolIndex, 0, npc.patrolRoute.Count - 1);
    }

    public void ExitState(NPCBase npc) { }
}

// ----------------------------------------------------------------------
// 2. WAIT STATE (Folosește waitTime din structură)
// ----------------------------------------------------------------------
public class WaitState : INPCState
{
    public NPCBase.NPCStateID StateID => NPCBase.NPCStateID.Wait;
    
    private float timer;
    private float duration;

    public void SetDuration(float d) => duration = d;

    public void EnterState(NPCBase npc)
    {
        timer = 0f;
        if (npc.Agent.isOnNavMesh)
        {
            npc.Agent.isStopped = true;
            npc.Agent.ResetPath();
        }
    }

    public void DoState(NPCBase npc)
    {
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            npc.ToPatrol();
        }
    }

    public void ExitState(NPCBase npc)
    {
        if (npc.Agent.isOnNavMesh) npc.Agent.isStopped = false;
    }
}

// ----------------------------------------------------------------------
// 3. IDLE STATE (Fallback/Așteptare infinită)
// ----------------------------------------------------------------------
public class IdleState : INPCState
{
    public NPCBase.NPCStateID StateID => NPCBase.NPCStateID.Idle;

    public void EnterState(NPCBase npc)
    {
        if (npc.Agent.isOnNavMesh)
        {
            npc.Agent.isStopped = true;
            npc.Agent.ResetPath();
        }
    }

    public void DoState(NPCBase npc)
    {
        // În Idle-ul de stealth, inamicul doar stă. 
        // Poate fi întrerupt doar de un stimul extern (văz/auz).
    }

    public void ExitState(NPCBase npc) { }
}


public class ChaseState : INPCState
{
    public NPCBase.NPCStateID StateID => NPCBase.NPCStateID.Chase;

    private float boredomTimer;
    private Vector3 lastKnownPosition;
    private Vector3 lastMoveDirection;
    private float predictionForce = 1.2f; // Cât de mult anticipează mișcarea (secunde)

    public void EnterState(NPCBase npc)
    {
        if (npc.Agent.isOnNavMesh)
        {
            npc.Agent.isStopped = false;
            npc.Agent.speed = npc.data.runSpeed;
            // Ne asigurăm că nu are stopping distance mare pentru a nu se opri prea devreme
            npc.Agent.stoppingDistance = 1.5f; 
        }
        boredomTimer = 0f;
    }

    public void DoState(NPCBase npc)
    {
        if (npc.canSeePlayer)
        {
            // --- LOGICA DE ANTICIPARE (PREDICTION) ---
            boredomTimer = 0f;
            
            // Calculăm vectorul de mișcare al jucătorului
            // Dacă playerul are Rigidbody, folosim velocity. Dacă nu, calculăm diferența de poziție.
            Vector3 playerVelocity = Vector3.zero;
            Rigidbody rb = npc.playerTransform.GetComponent<Rigidbody>();
            
            if (rb != null) playerVelocity = rb.linearVelocity;
            
            // Memorăm direcția pentru când îl pierdem din vedere
            lastMoveDirection = playerVelocity.normalized;
            lastKnownPosition = npc.playerTransform.position;

            // Destinația este poziția actuală + unde va fi peste 'predictionForce' secunde
            Vector3 targetDestination = lastKnownPosition + (playerVelocity * predictionForce);

            // Verificăm dacă punctul anticipat este pe NavMesh, altfel mergem la poziția reală
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetDestination, out hit, 3.0f, NavMesh.AllAreas))
            {
                npc.Agent.SetDestination(hit.position);
            }
            else
            {
                npc.Agent.SetDestination(lastKnownPosition);
            }
        }
        else
        {
            // --- LOGICA DE MOMENTUM (CÂND TE-A PIERDUT) ---
            boredomTimer += Time.deltaTime;

            // Inamicul nu merge doar la ultimul punct văzut, ci încearcă să "anticipeze" coridorul
            // Merge spre ultima poziție + încă un pic în direcția în care fugeai
            Vector3 searchPoint = lastKnownPosition + (lastMoveDirection * 2f);
            npc.Agent.SetDestination(searchPoint);

            // Dacă a ajuns la punctul de căutare și tot nu te vede
            if (npc.Agent.remainingDistance <= npc.Agent.stoppingDistance + 0.5f)
            {
                // Aici ar putea începe o animație de "uita-te stânga-dreapta"
                if (boredomTimer >= npc.data.chaseBoredomTime)
                {
                    npc.currentDetection = 0f;
                    npc.ToPatrol();
                }
            }
        }
    }

    public void ExitState(NPCBase npc)
    {
        if (npc.Agent.isOnNavMesh)
        {
            npc.Agent.speed = npc.data.walkSpeed;
            npc.Agent.stoppingDistance = 0.5f; // Resetăm la valoarea de patrulare
        }
    }
}