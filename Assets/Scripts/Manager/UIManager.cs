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
    [SerializeField] GameObject CharacterMenuPanel;
    [SerializeField] GameObject CharacterStatsUIPanel;
    [SerializeField] GameObject helpPanel;
    GameObject currentOpenMenu = null;
    [SerializeField] CharacterMenu characterMenu;   
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
        OpenHelp();
        UpdateExp(1, 0, 40);
        UpdateGold(0);
    }
    //sets the bottom skill UI's to the current leaders skills
    public void SetSkillUI()
    {
        for (int i = 0; i < 3; ++i)
        {
            skillUI[i].ChangeBlock();
        }
    }
    //sets the UI for the left side of the screen
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
    //sets the UI for the main menu that apepars when esc is pressed
    public void setMenuUI()
    {
        for (int i = 0; i < 4; ++i)
        {
            playerMenuUIArray[i].SetStats(partyManager.memberRef[partyManager.partyMembers[i]].playerNavScript.GetCharacterData());
        }
    }
    public void UpdateExp(int _level, float _currentExp, float _requiredExp)
    {
        characterMenu.UpdateExpLevel(_level, _currentExp, _requiredExp);
    }
    public void UpdateGold(int gold)
    {
        characterMenu.UpdateGold(gold);
    }
    public void CloseQuestPanel()
    {
        if (menuIsOpen && currentOpenMenu == questPanel)
        {
            menuIsOpen = false;
            questPanel.SetActive(false);
            currentOpenMenu = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!menuIsOpen && currentOpenMenu == null)
            {
                menuIsOpen = true;
                questPanel.SetActive(true);
                currentOpenMenu = questPanel;
            }
            else if (menuIsOpen && currentOpenMenu == questPanel)
            {
                CloseQuestPanel();
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //check if a menu is open
            if (menuIsOpen)
            {
                //if the player is on the main menu, disable the menu
                if (currentOpenMenu == CharacterMenuPanel)
                {
                    menuIsOpen = false;
                    currentOpenMenu.SetActive(false);
                    currentOpenMenu = null;
                }
                if (currentOpenMenu == questPanel)
                {
                    menuIsOpen = false;
                    questPanel.SetActive(false);
                    currentOpenMenu = null;
                }
                if (currentOpenMenu == CharacterStatsUIPanel)
                {
                    BackToMenu();
                }                
               if (currentOpenMenu == helpPanel)
                {
                    CloseHelp();
                }
            }
            //if no menu is open
            else {
                if (currentOpenMenu == null)
                {
                    menuIsOpen = true;
                    CharacterMenuPanel.SetActive(true);
                    currentOpenMenu = CharacterMenuPanel;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!menuIsOpen && currentOpenMenu == null) {
                OpenHelp();            
            }
        }
    }
    public void ExitMenu()
    {
        //check if a menu is open
        if (menuIsOpen)
        {
            //if the player is on the main menu, disable the menu
            if (currentOpenMenu == CharacterMenuPanel)
            {
                menuIsOpen = false;
                currentOpenMenu.SetActive(false);
                currentOpenMenu = null;
            }
        }
    }
    public void BackToMenu()
    {
        currentOpenMenu.SetActive(false);
        CharacterMenuPanel.SetActive(true);
        currentOpenMenu = CharacterMenuPanel;
    }
    public void OpenStatMenu()
    {
        currentOpenMenu.SetActive(false);
        CharacterStatsUIPanel.SetActive(true);
        currentOpenMenu = CharacterStatsUIPanel;
    }

    public void OpenHelp()
    {
        helpPanel.SetActive(true);
        currentOpenMenu = helpPanel;
        menuIsOpen = true;
    }
    public void CloseHelp()
    {
        menuIsOpen = false;
        helpPanel.SetActive(false);
        currentOpenMenu = null;
    }
}
