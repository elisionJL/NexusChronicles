using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyType : ScriptableObject
{    
        //base stats are the stats at level 1, max stats are the stats at max level aka level 100
        public int baseHp, baseAtk, MaxHp, MaxAtk;
        public float range;
        public float attackSpeed;
    public enum ENEMY
    {
        MUSHROOM,
        WOLF,
        TOTAL
    }
    public ENEMY typeOfEnemy;
}
