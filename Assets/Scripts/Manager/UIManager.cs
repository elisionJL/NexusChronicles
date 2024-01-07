using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField ]PlayerUI[] playerDisplayUIArray = new PlayerUI[4];
    [SerializeField] PlayerMenuUI[] playerMenuUIArray = new PlayerMenuUI[4];
    PartyManager partyManager;
    public bool menuIsOpen = false;
    [SerializeField]GameObject questPanel;
    [SerializeField]SkillUIBlock[] skillUI = new SkillUIBlock[3];
    [SerializeField] GameObject MenuCharacterUIPanel;
    [SerializeField] GameObject CharacterStatsUIPanel;
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
    void Start()
    {
        partyManager = PartyManager.instance;
        for (int i = 0; i < 3; ++i)
        {
            skillUI[i].ChangeBlock();
        }
    }
    public void SetPartyUI()
    {
        if(partyManager == null)
        {
            partyManager = PartyManager.instance;
        }
        for (int i = 0; i < 4; ++i)
        {
            playerDisplayUIArray[i].SetStats(partyManager.memberRef[partyManager.partyMembers[i]].playerNavScript.GetCharacterData());
        }
    }
    public void setMenuUI()
    {
        for (int i = 0; i < 4; ++i)
        {
            playerMenuUIArray[i].SetStats(partyManager.memberRef[partyManager.partyMembers[i]].playerNavScript.GetCharacterData());
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!menuIsOpen)
            {
                menuIsOpen = true;
                questPanel.SetActive(true);
            }
            else
            {
                menuIsOpen = false;
                questPanel.SetActive(false);
            }
        }
    }
}
