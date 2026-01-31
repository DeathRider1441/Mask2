using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[System.Serializable]
public struct PatrolPoint
{
    public Transform point;
    public float waitTime;
}

public abstract class NPCBase : Entity
{
    [Header("Movement")]
    [SerializeField] private float defaultSpeed = 3.5f;
    public NavMeshAgent Agent { get; private set; }

    [Header("Stealth Patrol")]
    public List<PatrolPoint> patrolRoute = new List<PatrolPoint>();
    [HideInInspector] public int currentPatrolIndex = 0;
    [HideInInspector] public bool isForward = true;

    // State Machine
    public enum NPCStateID { Patrol, Wait, Idle, Dead, Chase }
    public INPCState currentState;
    public NPCStateID CurrentStateID { get; private set; }
    
    public Animator animator;
    private bool isChangingState = false;

    // Instanțe Stări
    public PatrolState patrolState = new PatrolState();
    public WaitState waitState = new WaitState();
    public ChaseState chaseState = new ChaseState();

    protected virtual void Awake()
    {
        base.Awake();
        Agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start(); // Din Entity: Health & Visuals
        
        if (Agent != null) Agent.speed = defaultSpeed;

        // Pornim patrularea dacă avem traseu
        if (patrolRoute.Count > 0) ToPatrol();
        else ToIdle();
    }

    public void ChangeState(INPCState newState)
    {
        if (isChangingState || currentState == newState) return;
        if (Agent == null || !Agent.enabled) return;

        isChangingState = true;
        currentState?.ExitState(this);
        currentState = newState;
        CurrentStateID = newState.StateID;
        currentState.EnterState(this);
        isChangingState = false;
    }

    protected override void Update()
    {
        if (Agent == null || !Agent.isOnNavMesh) return;
        
        base.Update(); 
        currentState?.DoState(this);

        // Sync Animator
        if (animator != null)
        {
            animator.SetInteger("State", (int)CurrentStateID);
        }
    }

    protected override void OnPlayerDetected()
    {
        base.OnPlayerDetected();
        ToChase();
    }

    // --- Tranziții ---
    public virtual void ToPatrol() => ChangeState(patrolState);
    public virtual void ToWait(float duration) 
    {
        waitState.SetDuration(duration);
        ChangeState(waitState);
    }
    public void ToIdle()
    {
        if (Agent.isOnNavMesh) Agent.isStopped = true;
        // O stare de fallback dacă nu are puncte
    }
    public void ToChase() => ChangeState(chaseState);

    // === LOGICĂ DE MOARTE (OVERRIDE) ===
    protected override void Die()
    {
        // 1. Executăm logica de bază din Entity (Loot, IsDead = true, Salvare stare)
        base.Die();

        // 2. Oprim AI-ul imediat
        if (Agent != null)
        {
            Agent.isStopped = true;
            Agent.enabled = false;
        }

        // 3. Feedback vizual/animație
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Asigură-te că ai un Trigger "Die" în Animator
            animator.SetInteger("State", (int)NPCStateID.Dead);
        }

        Debug.Log($"<color=red>[Stealth]</color> {gameObject.name} a fost eliminat.");
        
        // Dacă ai Ragdoll sau vrei să lași corpul pe jos, poți opri Destroy-ul aici
        // sau poți lăsa base.Die() să se ocupe de distrugere după un delay.
    }
}