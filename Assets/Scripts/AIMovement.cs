using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIMovement : MonoBehaviour
{
    Transform leader,partyPos;
    NavMeshAgent nav;
    Animator animator;
    CharacterData stats; 
    //this sections hold the variables used in combat
    #region Combat
    CombatManager combatManager;

    #endregion Combat
    // Start is called before the first frame update
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        if (nav == null)
        {
            nav = transform.parent.GetComponent<NavMeshAgent>();
        }
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        combatManager = CombatManager.instance;
        stats = GetComponent<CharacterData>();
    }
    public void SetLeader(Transform lead,Transform pos)
    {
        nav.isStopped = false;
        leader = lead;
        partyPos = pos;
        if (leader != null)
        {
            //will need some way to spawn them on a hill
            transform.position = partyPos.position;
            //look at party leader
            transform.rotation = Quaternion.LookRotation(leader.position - transform.position);
        }
    }
    public void StopNavigation()
    {
        nav.isStopped = true;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //character cannot move when downed and in the middle of usign their skill and attack
        if (stats.currentState < CharacterData.STATES.ATTACKING)
        {
            GetMovement();
        }
        if (stats.hp <= 0)
        {
            stats.currentState = CharacterData.STATES.DOWNED;
            return;
        }
        //the time between attacks
        if (stats.attackTimer < stats.attackspeed && stats.currentState < CharacterData.STATES.ATTACKING)
        {
            stats.attackTimer += Time.deltaTime;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && stats.currentState == CharacterData.STATES.ATTACKING)
        {
            stats.currentState = CharacterData.STATES.IDLING;
        }
        if (combatManager.inCombat)
        {
            EnemyData target = combatManager.targettedEnemy;
            if ((target.GetPos() - transform.position).sqrMagnitude < stats.sqrRange + target.GetWidth() && stats.attackTimer >= stats.attackspeed)
            {
                if (stats.currentState != CharacterData.STATES.UNLEASH && stats.currentState != CharacterData.STATES.ATTACKING)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (stats.skillTimer[i] <= 0)
                        {
                            stats.currentState = CharacterData.STATES.UNLEASH;
                            UseSkill(i);
                            break;
                        }
                    }
                }
                if (stats.currentState == CharacterData.STATES.IDLING)
                {
                    stats.currentState = CharacterData.STATES.ATTACKING;
                    FaceEnemy();
                    animator.SetTrigger("Trigger");
                    animator.SetInteger("Trigger Number", 2);
                }
            }
            //this means that the attack animation is done so set back the state to idle

        }
    }
    public void GetMovement()
    {
        if(!combatManager.inCombat)
            nav.SetDestination(partyPos.position);
        else
        {
            EnemyData target = combatManager.targettedEnemy;
            Vector3 direction = target.GetPos() - transform.position;
            direction.y = 0;
            float sqrDist = direction.sqrMagnitude;
            Vector3 characterToEnemyDirection = direction.normalized;
            switch (stats.characterRole.characterClass)
            {
                case Role.ROLE.DEFENDER:
                case Role.ROLE.ATTACKER:
                    //if they are outside of the attack range than move closer
                    //if the character is too close than move back
                    if (sqrDist > stats.sqrRange + target.GetWidth() - 4 || sqrDist < target.GetWidth() + 2)
                    {
                        nav.SetDestination(target.GetPos() - (characterToEnemyDirection * (stats.range -1)));
                    }
                    else
                    {
                        nav.SetDestination(transform.position);
                    }
                    break;
                case Role.ROLE.HEALER:
                    //if they are outside of the attack range than move closer
                    //if the character is too close than move back
                    if (sqrDist > stats.sqrRange + target.GetWidth() - 4 || sqrDist < target.GetWidth() + 4)
                    {
                        nav.SetDestination(target.GetPos() - (characterToEnemyDirection * (stats.range - 1)));
                    }
                    else
                    {
                        nav.SetDestination(transform.position);
                    }
                    break;
                default:
                    break;
            }
        }
        if (nav.remainingDistance > 0.1)
        {
            if (nav.remainingDistance > 10)
            {
                transform.position = partyPos.position;
            }
            animator.SetBool("Moving", true);
            animator.SetFloat("Velocity", 1);
            stats.currentState = CharacterData.STATES.MOVING;
        }
        else
        {
            animator.SetBool("Moving", false);
            animator.SetFloat("Velocity", 0);
            stats.currentState = CharacterData.STATES.IDLING;
        }
    }
    void FaceEnemy()
    {
        Vector3 direction = combatManager.targettedEnemy.GetPos() - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    void UseSkill(int skillNum)
    {
        if (stats.GetUsable(skillNum))
        {
            EnemyData target = combatManager.targettedEnemy;
            FaceEnemy();
            stats.currentState = CharacterData.STATES.UNLEASH;

            switch (skillNum)
            {
                case 0:
                    stats.Skill1(target);
                    break;
                case 1:
                    stats.Skill2(target);
                    break;
                case 2:
                    stats.Skill3(target);
                    break;
            }

            animator.SetTrigger("Trigger");
            animator.SetInteger("Trigger Number", 2);
        }
    }
}
