using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    //level of the charcater
    [SerializeField]int level = 1;
    //the characters hp and atk accounting their base stats and their level
    //create a seperate variable here so that we can increase or decrease it as we like and can reset to level hp and atk easily
    public float levelHp, levelAtk, hp, atk;
    public float attackspeed = 1;
    public float attackTimer = 0;
    public float sqrRange = 3;
    public float range = 1;
    [SerializeField]MeshRenderer meshRenderer;
    [SerializeField] SkinnedMeshRenderer sMeshRenderer;
    float sideScale;
    [SerializeField] EnemyType enemyType;
    public EnemyAI enemyAI;
    public enum STATES
    {
        IDLING = 0,
        MOVING = 1,
        REVIVING,
        GUARDING,
        ATTACKING,
        UNLEASH,
        DOWNED
    }
    public STATES currentState = STATES.IDLING;
    // Start is called before the first frame update
    void Start()
    {
        levelHp = enemyType.baseHp + (enemyType.MaxHp - enemyType.baseHp) * (level - 1) / 99;
        levelAtk = enemyType.baseAtk + (enemyType.MaxAtk - enemyType.baseAtk) * (level - 1) / 99;
        atk = levelAtk;
        hp = levelHp;
        sqrRange = Mathf.Pow(enemyType.range, 2);
        range = enemyType.range;
        attackspeed = enemyType.attackSpeed;
        //meshRenderer = GetComponent<MeshRenderer>();
        enemyAI.GetComponent<EnemyAI>();
        if(meshRenderer == null)
        {
            sideScale = Mathf.Pow(sMeshRenderer.bounds.size.x * 0.5f, 2);
        }
        else
            sideScale = Mathf.Pow(meshRenderer.bounds.size.x * 0.5f, 2);
    }
    public void loseHp(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            CombatManager.instance.RemoveEnemy(transform);
        }
    }
    public EnemyType.ENEMY GetEnemyType()
    {
        return enemyType.typeOfEnemy;
    }
    public Vector3 GetPos()
    {
        return transform.position;
    }
    public float GetHeight()
    {
        if (meshRenderer == null)
        {
            return sMeshRenderer.bounds.size.y;
        }
        else
        {
            return meshRenderer.bounds.size.y;
        }
    }
    public void StartDeath()
    {
        enemyAI.StartDeathAnim();
    }
    public float GetWidth()
    {
        return sideScale;
    }
    public Vector3 GetForward()
    {
        return transform.forward;
    }
    public Vector3 GetXZForward()
    {
        return new Vector3(transform.forward.x, 0, transform.forward.z);
    }
    // Update is called once per frame
    void Update()
    {
        if(attackTimer  < attackspeed)
        {
            attackTimer += Time.deltaTime;
        }
    }

}
