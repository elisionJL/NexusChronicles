using UnityEngine;
using System.Collections.Generic;
public class PlayerController : MonoBehaviour
{
    Animator animator;
    public Transform cameraTrans;
    float speed = 5;
    Vector3 forward;
    //this is the look at gameobject that is ON THE CHARACTERS
    [SerializeField] Transform camLookAtTarget;
    CharacterData stats;
    //this sections hold the variables used in combat
    #region Combat
    CombatManager combatManager;

    private void Awake()
    {
        stats = GetComponent<CharacterData>();
        animator = GetComponent<Animator>();
    }
    #endregion Combat
    // Start is called before the first frame update
    void Start()
    {
        combatManager = CombatManager.instance;
        SetNormalCamera();
    }
    private void OnDisable()
    {
        combatManager = CombatManager.instance;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //character cannot move when downed and in the middle of usign their skill and attack
        if (stats.currentState <= CharacterData.STATES.MOVING)
        {
            if (UIManager.instance.menuIsOpen)
                return;
            GetMovementInput();
        }
        //the time between attacks
        if(stats.attackTimer < stats.attackspeed && stats.currentState < CharacterData.STATES.ATTACKING)
        {
            stats.attackTimer += Time.deltaTime;
        }
        if (combatManager.inCombat)
        {
            if(stats.hp <= 0)
            {
                stats.currentState = CharacterData.STATES.DOWNED;
                return;
            }
            EnemyData target = combatManager.targettedEnemy;
            if (Input.GetKeyDown(KeyCode.Alpha1) && stats.currentState <= CharacterData.STATES.MOVING )
            {
                UseSkill(0);


            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && stats.currentState <= CharacterData.STATES.MOVING)
            {
                UseSkill(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && stats.currentState <= CharacterData.STATES.MOVING)
            {
                UseSkill(2);
            }
            //check if the player can use an auto attack
            if (stats.attackTimer >= stats.attackspeed && stats.currentState == CharacterData.STATES.IDLING)
            {
                if ((target.GetPos() - transform.position).sqrMagnitude < stats.sqrRange + target.GetWidth())
                {
                    FaceEnemy();
                    stats.currentState = CharacterData.STATES.ATTACKING;
                    animator.SetTrigger("Trigger");
                    animator.SetInteger("Trigger Number", 2);

                }
            }
        }
    }
    
    void GetMovementInput()
    {
        //get input
        float valueX = Input.GetAxisRaw("Horizontal");
        float valueY = Input.GetAxisRaw("Vertical");
        //if no moving set to idle
        if (valueX == 0 && valueY == 0)
        {
            stats.currentState = CharacterData.STATES.IDLING;
            animator.SetBool("Moving", false);
            animator.SetFloat("Velocity", 0);
            return;
        }
        //else set state to moving and trigger move animation
        else
        {
            stats.currentState = CharacterData.STATES.MOVING;
            animator.SetBool("Moving", true);
            animator.SetFloat("Velocity", 1);
        }
        //move player in respect to the camera
        //get the forward vector of the camera relative of the character
        forward = (transform.position - cameraTrans.position).normalized;
        forward.y = 0;
        //get the right vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        //the movement vector
        Vector3 direction = (valueX * right) + (valueY * forward);
        //look at where they are moving to
        transform.rotation = Quaternion.LookRotation(direction.normalized);
        //move
        transform.position += direction.normalized * Time.deltaTime * speed;
    }
    //try to use the skill
    //skill num refers to the skill index in the CharacterData skill array;
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
    //return the look at target on the current camera
    public Transform getLookAt()
    {
        return camLookAtTarget;
    }
    //makes the character face the enemy
    void FaceEnemy()
    {
        Vector3 direction = combatManager.targettedEnemy.GetPos() - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    //get CharcaterData of the current character
    public CharacterData GetCharacterData()
    {
        return stats;
    }
    //changes the camera used to the combat camera
    public void SetCombatCamera()
    {
        cameraTrans = PartyManager.instance.GetPlayerCombatCameraTransform();
    }
    //changes the camera used to the movement camera
    public void SetNormalCamera()
    {
        cameraTrans = PartyManager.instance.GetPlayerCameraTransform();
    }

}
