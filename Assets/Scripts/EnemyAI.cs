using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    [SerializeField]Transform follow;
    [SerializeField] bool isLeader = false;
    CombatManager combatManager;
    PartyManager partyManager;
    public EnemyData stats;
    NavMeshAgent navAgent;
    public bool inRangeOfPlayer = false;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        combatManager = CombatManager.instance;
     
        stats = GetComponent<EnemyData>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        partyManager = PartyManager.instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        if (inRangeOfPlayer)
        {
            Attack();
        }
    }
    void Movement()
    {
        if (inRangeOfPlayer)
        {
            CharacterData target = partyManager.memberRef[combatManager.GetHighestAggro()].characterData;
            Vector3 direction = target.GetPos() - transform.position;
            direction.y = 0;
            float sqrDist = direction.sqrMagnitude;
            Vector3 characterToEnemyDirection = direction.normalized;
            if (sqrDist > stats.sqrRange - 4 || sqrDist < 2)
            {
                navAgent.SetDestination(target.GetPos() - (characterToEnemyDirection * (stats.range - 1)));
                animator.SetBool("Moving", true);
            }
            else
            {
                navAgent.SetDestination(transform.position);
                animator.SetBool("Moving", false);
            }
        }
        else
        {
            if ((follow.position - transform.position).magnitude > 0.1)
            {
                navAgent.SetDestination(follow.position);
                animator.SetBool("Moving", true);
            }
            else
            {
                navAgent.SetDestination(transform.position);
                animator.SetBool("Moving", false);
            }
        }
    }
    void Attack()
    {
        CharacterData target = partyManager.memberRef[combatManager.GetHighestAggro()].characterData;
        Vector3 direction = target.GetPos() - transform.position;
        direction.y = 0;
        float sqrDist = direction.sqrMagnitude;
        //Vector3 characterToEnemyDirection = direction.normalized;
        if (sqrDist < stats.sqrRange )
        {
            if (stats.attackTimer >= stats.attackspeed && stats.currentState == EnemyData.STATES.IDLING)
            {
                stats.currentState = EnemyData.STATES.ATTACKING;
                FaceTarget(direction);
                animator.SetTrigger("Attack");
                stats.attackTimer = 0;
            }
        }
    }
    //make the model face the target;
    void FaceTarget(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }
    public void StartDeathAnim()
    {
        animator.SetBool("Dead", true);
    }
    //animation event
    public void Hit()
    {
        CharacterData target = partyManager.memberRef[combatManager.GetHighestAggro()].characterData;
        if (target == null)
            return;
        target.LoseHealth(stats.atk);
    }
    public void HitEnd()
    {
       stats.currentState = EnemyData.STATES.IDLING;

    }
    //destroy it self after death animation plays
    public void DeathAnim()
    {
        Destroy(gameObject);
    }
}
