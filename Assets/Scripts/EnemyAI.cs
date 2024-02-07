using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    [SerializeField]Transform follow;
    WanderScript wanderScript;
    Vector3 originPos;
    [SerializeField] bool isLeader = false;
    CombatManager combatManager;
    PartyManager partyManager;
    public EnemyData stats;
    NavMeshAgent navAgent;
    public bool inRangeOfPlayer = false;
    Animator animator;
    LineRenderer lineRenderer;
    //this is how many vertexes the line renderer will use
    [SerializeField] int linePointCount = 100;
    //lerp is the count used when lerping between the 2 objects
    //half is used to start going down once the half point is reached;
    [SerializeField]int linePointCountHalf;
    //the how much higher than the target the taller obejct should be;
    [SerializeField] float lineHeightIncrease = 1;
    public CharacterData currentTarget = null;
    public bool returning = false;


    // Start is called before the first frame update
    void Start()
    {
        combatManager = CombatManager.instance;
     
        stats = GetComponent<EnemyData>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        partyManager = PartyManager.instance;
        //floors the result
        linePointCountHalf = linePointCount/2;
        lineRenderer.positionCount = linePointCountHalf;
        wanderScript = follow.GetComponent<WanderScript>();
        originPos = wanderScript.wanderOrigin.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        //check if the player has entered the range
        CheckRangeFromPlayer();
        if (inRangeOfPlayer)
        {
            Attack();
            DrawAggroLine();
            CheckRangefromOrigin();
        }
        else if (returning)
        {
            if ((follow.position - transform.position).sqrMagnitude < 1)
            {
                Debug.Log("setted: " + gameObject);
                returning = false;
                inRangeOfPlayer = false;
            }
        }
    }
    void CheckRangeFromPlayer()
    {
        if (returning)
            return;

        Vector3 targetPos = partyManager.GetLeaderTransform().position;
        targetPos = transform.position - targetPos;
        targetPos.y = 0;
        if (inRangeOfPlayer == false)
        {
            //Player Enters the range
            if (targetPos.sqrMagnitude <= stats.sqrRange)
            {
                inRangeOfPlayer = true;
                combatManager.EnemyEntersRange(stats);
                EnableAggroLine();
            }
        }
        //else if (inRangeOfPlayer == true)
        //{
        //    //Player exits the range
        //    if (targetPos.sqrMagnitude >= stats.sqrRange )
        //    {
        //        inRangeOfPlayer = false;
        //        combatManager.EnemyExitsRange(stats);
        //    }
        //}
    }
    void CheckRangefromOrigin()
    {
        //Check if enemy is too far from origin
        Vector3 originDirection = originPos - transform.position;
        originDirection.y = 0;
        float distanceFromOrigin = originDirection.sqrMagnitude;
        //too far from origin
        if (distanceFromOrigin > 144 && returning == false)
        {
            returning = true;
            DisableAggroLine();
            //Debug.Log("Too far: " + gameObject);
            combatManager.EnemyExitsRange(stats);
        }
        if (returning == false)
            return;
        //
        if ((follow.position - transform.position).sqrMagnitude > 1)
        {
            navAgent.SetDestination(follow.position);
            animator.SetBool("Moving", true);
        }
        else
        {
            returning = false;
            inRangeOfPlayer = false;
        }
    }
    void Movement()
    {
        if (inRangeOfPlayer && !returning)
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
        else if (!returning)
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
        currentTarget = target;
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
    void DrawAggroLine()
    {
        Vector3 enemyPos = transform.position;
        Vector3 targetPos = currentTarget.GetLookAtPos();
        Vector3 frontVertexPosition,backVertexPosition;
        //find the vector diff between the enemy to the target;
        Vector3 difference = targetPos - enemyPos;
        //the max height of the arc
        float maxY;
        //check if enemy is higher than target
        //the arc height must be higher than the target;
        if(difference.y > 0)
        {
            maxY = targetPos.y + lineHeightIncrease;
        }
        //else the enemy is lower than the target;
        //the arc height must be heigher than the enemy
        else
        {
            maxY = enemyPos.y + lineHeightIncrease;
        }
        //caulculate the middle position at the max of the curve
        Vector3 middleVertexPosition = Vector3.Lerp(enemyPos, targetPos, 0.5f);
        //set the height
        middleVertexPosition.y = maxY;
        for (int i = 0; i < linePointCountHalf; ++i)
        {
            //convert i to float as we want a number that is between 1 and 0, int/int will auto round to closest whole thus nothing will change
            float time = (float)i / (linePointCountHalf -1);
            //this is to get the X and Z position
            //calculate front half
            frontVertexPosition = Vector3.Lerp(enemyPos, middleVertexPosition, time);
            //calculate back half
            backVertexPosition = Vector3.Lerp(middleVertexPosition, targetPos, time);
            //calculate final position to form a curve
            lineRenderer.SetPosition(i, Vector3.Lerp(frontVertexPosition, backVertexPosition, time));
        }
    }
    public void DisableAggroLine()
    {
        lineRenderer.enabled = false;
    }
    public void EnableAggroLine()
    {
        lineRenderer.enabled = true;
    }
    public void StartDeathAnim()
    {
        animator.SetBool("Dead", true);
    }
    //animation event
    public void Hit()
    {
        currentTarget = partyManager.memberRef[combatManager.GetHighestAggro()].characterData;
        if (currentTarget == null)
            return;
        currentTarget.LoseHealth(stats.atk);
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
