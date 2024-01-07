using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Role", menuName ="Role")]
public class Role : ScriptableObject
{
    public enum ROLE
    {
        ATTACKER,
        DEFENDER,
        HEALER,
        TOTAL
    }
    //base stats are the stats at level 1, max stats are the stats at max level aka level 100
    public int baseHp, baseAtk,MaxHp,MaxAtk;
    public float range;
    public float attackSpeed;
    public ROLE characterClass;
    public string roleName;
    public float damageReduction;
    public Skill[] skillList = new Skill[3];
}
