using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PartyManager : MonoBehaviour
{
    public static PartyManager instance { get; private set; }
    public Transform[] partyMembers = new Transform[4];
    public Transform[] partyPos = new Transform[4];
    [SerializeField] Transform leader;
    [SerializeField] CinemachineFreeLook playerCam;
    [SerializeField] CinemachineVirtualCamera playerCombatCam;
    [SerializeField] Transform MinimapPosition;
    public Dictionary<Transform, CharacterScripts> memberRef = new Dictionary<Transform, CharacterScripts>();

    public float recenteringTime = 5;
    float exp,expRequired;
    int gold ;
    int level;
    //debug purposes
    int count = 0;
    //just a container for the scripts in each character object
    public class CharacterScripts
    {
        public AIMovement navScript;
        public PlayerController playerNavScript;
        public CharacterData characterData;
        public CharacterScripts(AIMovement nav, PlayerController pNav,CharacterData data)
        {
            navScript = nav;
            playerNavScript = pNav;
            characterData = data;
        }
        public void IsLeader()
        {
            navScript.StopNavigation();
            navScript.enabled = false;
            playerNavScript.enabled = true;
        }
        public void IsNPC(int pos)
        {
            navScript.enabled = true;
            playerNavScript.enabled = false;
            //pos = 0 is the leader position
            navScript.SetLeader(instance.leader,instance.partyPos[pos]);
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else { 
            instance = this;
            exp = 0;
            expRequired = 40;
            gold = 0;
            level = 1;
        }

    }
    //on object initial activation
    private void Start()
    {
        for (int i = 0; i < partyMembers.Length; ++i)
        {
            Transform member = partyMembers[i];
            memberRef.Add(member,
                new CharacterScripts(
                    member.GetComponent<AIMovement>(),
                    member.GetComponent<PlayerController>(),
                    member.GetComponent<CharacterData>()
                    )
                );
        }
        //make sure the leader is 
        SwitchLeader();
        //set the hp UI during gameplay
        UIManager.instance.SetPartyUI();
        //set the UI for when menu is opened
        UIManager.instance.setMenuUI();
        //initialise the aggroo values 
        CombatManager.instance.StartAggro();


    }

    public void ResetAllSkillCD()
    {
        for (int i = 0; i < partyMembers.Length; ++i)
        {
            Transform member = partyMembers[i];
            memberRef[member].characterData.ResetSkillsCD();
        }
    }
    //makes the camera follow the leader and enable the AI on the other part members
    public void SwitchLeader()
    {
        int count = 1;
        for (int i = 0; i < partyMembers.Length; ++i)
        {
            Transform member = partyMembers[i];
            //Check if the charcater is the leader we want
            if(member == leader)
            {
                //set the camera to follow and look at
                playerCam.Follow = leader;
                //need to get the lookat object in the charcater
                playerCam.LookAt = memberRef[leader].playerNavScript.getLookAt();
                //Disable the Ai script and enable the player input script
                memberRef[leader].IsLeader();
                //change the follow positions to follow the leader
                partyPos[0].parent = leader;
                partyPos[0].position = leader.position;
                partyPos[0].localRotation = Quaternion.identity;
            }
            else
            {
                //disable the player input script and enable the Ai script
                memberRef[member].IsNPC(count++);
            }
        }
    }
    //reset the state of all characters back to idle after fight ends
    //public void ResetState()
    //{
    //    for (int i = 0; i < partyMembers.Length; ++i)
    //    {
    //        if(partyMembers[i] != leader)
    //    }
    //}
    public Transform GetLeaderTransform()
    {
        return leader;
    }
    //camera functions
    public Transform GetPlayerCameraTransform()
    {
        return playerCam.transform;
    }
    public CinemachineFreeLook GetPlayerCamera()
    {
        return playerCam;
    }
    public Transform GetPlayerCombatCameraTransform()
    {
        SwitchCombatLookAt();
        return playerCombatCam.transform;
    }

    //Switch what the camera is looking at
    public void SwitchCombatLookAt()
    {
        playerCombatCam.LookAt = CombatManager.instance.targettedEnemy.transform;
    }

    //Toggle the camera between combat and non-Combat mode depending on the parameter
    public void ToggleCamera(bool inCombat)
    {
        playerCam.gameObject.SetActive(!inCombat);
        playerCombatCam.gameObject.SetActive(inCombat);
        if (inCombat)
        {
            memberRef[leader].playerNavScript.SetCombatCamera();
            playerCombatCam.Follow = memberRef[leader].playerNavScript.getLookAt();
        }
        else
        {
            memberRef[leader].playerNavScript.SetNormalCamera();
        }
    }

    //return the cinemachine freelook used by the player
    public CinemachineFreeLook GetPlayerCameraFreeLook(){
        return  playerCam;
    }

    //start the countdown for the camera reset
    public void EnableCameraReset()
    {
        if (recenteringTime > 0)
        {
            recenteringTime -= Time.deltaTime;
            if (recenteringTime <= 0)
            {
                playerCam.m_RecenterToTargetHeading.m_enabled = true;
                playerCam.m_YAxisRecentering.m_enabled = true;
                playerCam.m_RecenterToTargetHeading.RecenterNow();
                playerCam.m_YAxisRecentering.RecenterNow();
            }
        }
    }

    //cancel the camera reset if is mid reset and reset the timer;
    public void CancelCameraReset()
    {
        playerCam.m_YAxisRecentering.CancelRecentering();
        playerCam.m_RecenterToTargetHeading.CancelRecentering();
        playerCam.m_RecenterToTargetHeading.m_enabled = false;
        playerCam.m_YAxisRecentering.m_enabled = false;
        recenteringTime = 5;
    }

    //move the minimapcamera with with player
    public void MoveMinimap(Vector3 pos) 
    {
        pos.y = MinimapPosition.position.y;
        MinimapPosition.position = pos;
    }

    //adds exp to the Party and increase the level if needed
    public void AddExp(float _exp)
    {
        exp += _exp;
        if(exp >= expRequired)
        {
            exp = Mathf.Ceil(exp - expRequired);
            level = level + 1;
            expRequired = 100+ (level/2)*(20*level);
            //Debug.Log("exp:" + exp + " Required Exp: " + expRequired);
            for(int i = 0; i < partyMembers.Length; i++)
            {
                memberRef[partyMembers[i]].characterData.level = level;
                memberRef[partyMembers[i]].characterData.UpdateStats();
                memberRef[partyMembers[i]].characterData.SetTolevel();
            }
            AddExp(0);
        }
    }
    public void AddGold(int _gold)
    {
        gold += _gold;
    }
    public void UpdateMenu()
    {
        UIManager.instance.UpdateExp(level, exp, expRequired);
        UIManager.instance.UpdateGold(gold);
    }
    // Update is called once per frame
    void Update()
    {
        //press B
        if (Input.GetKeyDown(KeyCode.B))
        {
            count++;
            if (count == 4)
                count = 0;
            leader = partyMembers[count];
            SwitchLeader();
            UIManager.instance.SetSkillUI();
        }
        float valueX = Input.GetAxisRaw("Horizontal");
        float valueY = Input.GetAxisRaw("Vertical");
        float valueCamX = Input.GetAxisRaw("CamHorizontal");
        float valueCamY = Input.GetAxisRaw("CamVertical");
        if (valueCamX == 0 && valueCamY == 0 && valueX == 0 && valueY == 0)
        {
            EnableCameraReset();
        }
        else
        {
            CancelCameraReset();
        }

    }
}
