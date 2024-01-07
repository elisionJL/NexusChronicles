using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    #region Stats
    //level of the charcater
    int level = 1;
    //the characters hp and atk accounting their base stats and their level
    //create a seperate variable here so that we can increase or decrease it as we like and can reset to level hp and atk easily
    public float levelHp, LevelAtk,hp,atk;
    public float attackspeed = 1;
    public float attackTimer = 0;
    public float sqrRange = 3;
    public float range = 1;
    public bool dead = false;
    public float resistance = 1;
    public Role characterRole;
    public ParticleSystem[] skillParticleList = new ParticleSystem[3];
    public float[] skillTimer = new float[3];
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
    #endregion Stats

    public virtual void Awake()
    {
        //this forumla is taken from xenoblade chronicles 3 but instead of the max level used being 99 mine is 100
        levelHp = characterRole.baseHp + (characterRole.MaxHp - characterRole.baseHp) * (level - 1) / 99;
        LevelAtk = characterRole.baseAtk + (characterRole.MaxAtk - characterRole.baseAtk) * (level - 1) / 99;
        atk = LevelAtk;
        hp = levelHp;
        sqrRange = Mathf.Pow(characterRole.range, 2);
        range = characterRole.range;
        attackspeed = characterRole.attackSpeed;
        resistance = 1 - (characterRole.damageReduction / 100);
        for(int i = 0; i < characterRole.skillList.Length; ++i)
        {
            if(characterRole.skillList[i] == null)
            {
                continue;
            }
            skillParticleList[i] = characterRole.skillList[i].CreateParticle(transform);
            skillTimer[i] = 0;
        }
    }
    public void UpdateStats()
    {
        //this forumla is taken from xenoblade chronicles 3 but instead of the max level used being 99 mine is 100
        levelHp = characterRole.baseHp + (characterRole.MaxHp - characterRole.baseHp) * (level - 1) / 99;
        LevelAtk = characterRole.baseAtk + (characterRole.MaxAtk - characterRole.baseAtk) * (level - 1) / 99;
        switch (characterRole.characterClass)
        {
            case Role.ROLE.ATTACKER:
                attackspeed = 2;
                range = 1;
                break;
            case Role.ROLE.DEFENDER:
                attackspeed = 0;
                range = 1;
                break;
            case Role.ROLE.HEALER:
                attackspeed = 1;
                range = 1.5f;
                break;
            default:
                Debug.LogError("No Class assigned on " + gameObject);
                break;
        }
    }
    public void GainHealth(float heal)
    {
       
        hp += heal;
        if(hp > levelHp)
        {
            hp = levelHp;
        }
    }
    public void LoseHealth(float damage)
    {
        hp -= damage * resistance;
        if(hp < 0)
        {
            hp = 0;
        }
    }
    public bool GetUsable(int i)
    {
        if (skillTimer[i] > 0)
        {
            return false;
        }
        else
            return true;
    }
    public float GetMaxSkilLTime(int num)
    {
        return characterRole.skillList[num].GetTimer();
    }
    public string GetSkillText(int num)
    {
        return characterRole.skillList[num].GetDescription();
    }
    public bool Skill1(EnemyData enemy)
    {
        Skill skill = characterRole.skillList[0];
        if (skillTimer[0] > 0)
        {
            return false;
        }
        switch(skill.GetSkillTargetting())
        {
            case Skill.TARGETTING.SINGLE:
                skill.UseSkillSingle(atk, enemy, this, transform,skillParticleList[0]);              
                break;
            default:
                skill.UseSkillSingle(atk, enemy, this, transform, skillParticleList[0]);
                break;
        }
        skillTimer[0] = skill.GetTimer();
        return true;
    }
    public bool Skill2(EnemyData enemy)
    {
        Skill skill = characterRole.skillList[1];
        if (skillTimer[1] > 0)
        {
            return false;
        }
        switch (skill.GetSkillTargetting())
        {
            case Skill.TARGETTING.SINGLE:
                skill.UseSkillSingle(atk, enemy, this, transform, skillParticleList[1]);
                break;
            default:
                break;
        }
        skillTimer[1] = skill.GetTimer();
        return true;
    }
    public bool Skill3(EnemyData enemy)
    {
        Skill skill = characterRole.skillList[2];
        if (skillTimer[2] > 0)
        {
            return false;
        }
        switch (skill.GetSkillTargetting())
        {
            case Skill.TARGETTING.SINGLE:
                skill.UseSkillSingle(atk, enemy, this, transform, skillParticleList[2]);
                skillTimer[2] = skill.GetTimer();
                break;
            default:
                break;
        }
        skillTimer[2] = skill.GetTimer();
        return true;
    }
    public Vector3 GetPos()
    {
        return transform.position;
    }
    public void ResetSkillsCD()
    {
        for (int i = 0; i < 3; i++)
        {
            skillTimer[i] = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < 3; i ++)
        {
            if(characterRole.skillList[i] == null)
            {
                skillTimer[i] = 100;
            }   
            else if (skillTimer[i] > 0 && characterRole.skillList[i].cooldownUsingTime)
                skillTimer[i] -= Time.deltaTime;
        }
    }
    //needed to make the animations work
    public void FootR()
    {
        return;
    }
    public void FootL()
    {
        return;
    }
    //called mid animation of attack
    public void Hit()
    {
        EnemyData target = CombatManager.instance.targettedEnemy;
        //for if target dies mid hit
        if (target == null)
            return;
        target.loseHp(atk);
        attackTimer = 0;
    }
    //called at the end of the attack animation
    public void HitEnd()
    {
        if(!CombatManager.instance.inCombat && CombatManager.instance.enemiesInRangeOfPlayer.Count == 0)
        {
            hp = levelHp;
        }
        currentState = CharacterData.STATES.IDLING;
    }
}
