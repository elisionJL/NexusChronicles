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
    public Dictionary<Transform, CharacterScripts> memberRef = new Dictionary<Transform, CharacterScripts>();

    float recenteringTime = 5;
    //debug purposes
    int count = 0;
    //just a container for the scripts in each charcater object
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
    public void EnableCameraReset()
    {
        if (recenteringTime > 0)
        {
            recenteringTime -= Time.deltaTime;
            if (recenteringTime <= 0)
            {
                playerCam.m_RecenterToTargetHeading.RecenterNow();
                playerCam.m_RecenterToTargetHeading.m_enabled = true;
            }
        }
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
    public Transform GetPlayerCombatCameraTransform()
    {
        SwitchCombatLookAt();
        return playerCombatCam.transform;
    }
    public void SwitchCombatLookAt()
    {
        playerCombatCam.LookAt = CombatManager.instance.targettedEnemy.transform;
    }
    public void ToggleCamera(bool inCombat)
    {
        if(inCombat)
        {
            memberRef[leader].playerNavScript.SetCombatCamera();
        }
        else
        {
            memberRef[leader].playerNavScript.SetNormalCamera();
        }
        playerCam.gameObject.SetActive(!inCombat);
        playerCombatCam.gameObject.SetActive(inCombat);
    }
    public CinemachineFreeLook GetPlayerCameraFreeLook(){
        return  playerCam;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            count++;
            if (count == 4)
                count = 0;
            leader = partyMembers[count];
            SwitchLeader();
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
            recenteringTime = 5;
        }

    }
}
