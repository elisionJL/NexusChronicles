using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;
    PartyManager partyManager;
    public bool inCombat = false;
    public Dictionary<Transform, EnemyData> enemiesOnMap = new Dictionary<Transform, EnemyData>();
    public List<EnemyData> enemiesInRangeOfPlayer = new List<EnemyData>();
    public EnemyData targettedEnemy;
    [SerializeField] Transform enemyPointer;
    Dictionary<Transform, float> AggroList = new Dictionary<Transform, float>();
    QuestManager questManager;
    //List<Tra>
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        RegisterEnemiesOnMap();
        targettedEnemy = null;
        partyManager = PartyManager.instance;
        questManager = QuestManager.instance;
        Debug.Log("Registed Enemies on Start");
    }
    //a function that adds all the enemies in the current scene into the dictionary
    //this should only be called when a scene is initially loaded
    public void RegisterEnemiesOnMap() {
        //clear dictionary
        enemiesOnMap.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0; i <enemies.Length; i++)
        {
            Transform temp = enemies[i].transform;
            enemiesOnMap.Add(temp, temp.GetComponent<EnemyData>());
        }
    }

    //remove the enemey from the dictionary and make it so that they are not near the player
    //afterwards destroy the game object
    public void RemoveEnemy(Transform target)
    {
        EnemyData currentTarget = enemiesOnMap[target];
        questManager.UpdateQuestrogress(currentTarget.GetEnemyType(), 1);
        enemiesInRangeOfPlayer.Remove(enemiesOnMap[target]);
        enemiesOnMap.Remove(target);
        if (enemiesInRangeOfPlayer.Count == 0)
        {
            enemyPointer.SetParent(null, true);
            enemyPointer.gameObject.SetActive(false);
            targettedEnemy = null;
            enemiesInRangeOfPlayer.TrimExcess();
            if (inCombat)
            {
                inCombat = !inCombat;
                partyManager.ToggleCamera(inCombat);
            }
        }
        else
        {
            EnemyData temp = enemiesInRangeOfPlayer[0];
            SwitchEnemyPointer(temp);
        }
        currentTarget.StartDeath();
    
    }
    public void StartAggro()
    {
        if(partyManager == null)
        {
            partyManager = PartyManager.instance;
        }
        for(int i = 0;i < 4; i++)
        {
            AggroList.Add(partyManager.partyMembers[i], 10);
        }
    }
    public void ResetAggro()
    {
        for (int i = 0; i < 4; i++)
        {
            AggroList[partyManager.partyMembers[i]] = 10;
        }
    }
    public void ChangeAggro(Transform member, float aggroValue)
    {
        AggroList[member] += aggroValue;
    }
    public Transform GetHighestAggro()
    {
        Transform highest = null;
        float value = 0;
        for(int i = 0; i < 4; i++)
        {
            if(AggroList[partyManager.partyMembers[i]] > value)
            {
                value = AggroList[partyManager.partyMembers[i]];
                highest = partyManager.partyMembers[i];
            }
        }
        return highest;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && enemiesInRangeOfPlayer.Count != 0)
        {
            inCombat = !inCombat;
            partyManager.ToggleCamera(inCombat);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (targettedEnemy == null || enemiesInRangeOfPlayer.Count == 0)
                return;
            int index = enemiesInRangeOfPlayer.IndexOf(targettedEnemy);
            //if
            if (index + 1 <= enemiesInRangeOfPlayer.Count)
            {
                index++;
                //see if index is at end of list
                if(index == enemiesInRangeOfPlayer.Count)
                {
                    index = 0;
                }
                EnemyData temp = enemiesInRangeOfPlayer[index];
                SwitchEnemyPointer(temp);
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(enemiesInRangeOfPlayer.Count + " : " + enemiesInRangeOfPlayer.Capacity);
        }
    }
    void SwitchEnemyPointer(EnemyData temp)
    {
        targettedEnemy = temp;
        //Vector3 Pos = temp.GetPos();
        Vector3 Pos = Vector3.zero;
        Pos.y = temp.GetHeight() + 0.5f;
        enemyPointer.SetParent(temp.transform, true);
        enemyPointer.localPosition = Pos;
        if (inCombat)
            partyManager.SwitchCombatLookAt();
    }

    //Enemy calls this when Player goes into the range
    public void EnemyEntersRange(EnemyData temp)
    {
        if (enemiesInRangeOfPlayer.Contains(temp))
        {
            return;
        }
        if (enemiesInRangeOfPlayer.Count == 0)
        {
            enemyPointer.gameObject.SetActive(true);
            enemiesInRangeOfPlayer.Add(temp);
            SwitchEnemyPointer(temp);
        }
        else
            enemiesInRangeOfPlayer.Add(temp);
        temp.enemyAI.inRangeOfPlayer = true;
        temp.enemyAI.EnableAggroLine();
    }

    //Enemy Calls this when Player goes outside of the range
    public void EnemyExitsRange(EnemyData temp)
    {
        //Debug.Log("Enemy exits range: " + temp.gameObject);
        enemiesInRangeOfPlayer.Remove(temp);
        if (enemiesInRangeOfPlayer.Count == 0)
        {
            enemyPointer.SetParent(null, true);
            enemyPointer.gameObject.SetActive(false);
            targettedEnemy = null;
            enemiesInRangeOfPlayer.TrimExcess();
            ResetAggro();
            partyManager.ResetAllSkillCD();

            if (inCombat)
            {
                inCombat = !inCombat;
                partyManager.ToggleCamera(inCombat);
            }
        }
        else
        {
            temp = enemiesInRangeOfPlayer[0];
            SwitchEnemyPointer(temp);
        }
        //temp.enemyAI.inRangeOfPlayer = false;
        temp.enemyAI.DisableAggroLine();
        temp.enemyAI.currentTarget = null;
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        Debug.Log("Enemy Entered " + gameObject);

    //        EnemyData temp = enemiesOnMap[other.transform];
    //        //for some reason it sometimes adds the same game object into the list.
    //        if (enemiesInRangeOfPlayer.Contains(temp))
    //        {
    //            return;
    //        }
    //        if (enemiesInRangeOfPlayer.Count == 0)
    //        {
    //            enemyPointer.gameObject.SetActive(true);
    //            enemiesInRangeOfPlayer.Add(temp);
    //            SwitchEnemyPointer(temp);
    //        }
    //        else
    //            enemiesInRangeOfPlayer.Add(temp);
    //        temp.enemyAI.inRangeOfPlayer = true;
    //        temp.enemyAI.EnableAggroLine();
           
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        EnemyData temp = enemiesOnMap[other.transform];
    //        enemiesInRangeOfPlayer.Remove(temp);
    //        if (enemiesInRangeOfPlayer.Count == 0)
    //        {
    //            enemyPointer.SetParent(null, true);
    //            enemyPointer.gameObject.SetActive(false);
    //            targettedEnemy = null;
    //            enemiesInRangeOfPlayer.TrimExcess();
    //            ResetAggro();
    //            partyManager.ResetAllSkillCD();
                
    //            if (inCombat)
    //            {
    //                inCombat = !inCombat;
    //                partyManager.ToggleCamera(inCombat);
    //            }
    //        }
    //        else
    //        {
    //            temp = enemiesInRangeOfPlayer[0];
    //            SwitchEnemyPointer(temp);
    //        }
    //        temp.enemyAI.inRangeOfPlayer = false;
    //        temp.enemyAI.DisableAggroLine();
    //        temp.enemyAI.currentTarget = null;
    //    }
    //}
}
