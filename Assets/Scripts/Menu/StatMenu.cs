using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatMenu : MonoBehaviour
{
    //the portrait boxes at the top of the stat menu
    [SerializeField]PortraitBoxes[] characterPortraits = new PortraitBoxes[4];
    [SerializeField] TMP_Text cName,role, hp,  atk, def, range;
    [SerializeField] CharacterData[] characterDatas = new CharacterData[4];
    [SerializeField] GameObject[] selectHighlights = new GameObject[2];
    [System.Serializable]public enum Selection
    {
        EQUIPMENT,
        BACK
    }
    Selection selectIndex = Selection.EQUIPMENT;
    int characterIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i < characterDatas.Length; ++i)
        {
            characterPortraits[i].SetName(characterDatas[i].gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //go down the list
        if (Input.GetKeyUp(KeyCode.S))
        {
            selectHighlights[(int)selectIndex].SetActive(false);
            selectIndex =  ++selectIndex&Selection.BACK;
            selectHighlights[(int)selectIndex].SetActive(true);
        }
        //go up the list
        if (Input.GetKeyUp(KeyCode.W))
        {
            selectHighlights[(int)selectIndex].SetActive(false);
            selectIndex--;
            if (selectIndex < Selection.EQUIPMENT)
            {
                selectIndex = Selection.EQUIPMENT;
            }
            selectHighlights[(int)selectIndex].SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            GoLeft();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            GoRight();
        }
            if (Input.GetKeyUp(KeyCode.Return))
        {
            switch (selectIndex)
            {
                case Selection.BACK:
                    ButtonClick(selectIndex);
                    break;
                default:
                    break;
            }
        }
    }
    private void OnEnable()
    {
        characterPortraits[0].EnableHightlight();
        characterIndex = 0;
        selectHighlights[(int)selectIndex].SetActive(false);
        selectIndex = Selection.EQUIPMENT;
        selectHighlights[(int)selectIndex].SetActive(true);
    }
    public void GoRight()
    {
        //disable the highlight on the current portrait
        characterPortraits[characterIndex].DisableHightlight();
        //increase the increment and make sure it stays with 0-3
        characterIndex = ++characterIndex % 4;
        //enable new portrait highlight
        characterPortraits[characterIndex].EnableHightlight();
        ChangeStatsDisplay();
    }
    public void GoLeft()
    {
        //disable the highlight on the current portrait
        characterPortraits[characterIndex].DisableHightlight();
        //decrease the index
        characterIndex--;
        //check for negative
        if (characterIndex < 0)
            characterIndex = 3;
        //enable new portrait highlight
        characterPortraits[characterIndex].EnableHightlight();
        ChangeStatsDisplay();
    }
    public void ChangeStatsDisplay()
    {
        CharacterData cData = characterDatas[characterIndex];
        cName.text = cData.gameObject.name + " Lvl " + cData.level;
        hp.text = cData.levelHp + "";
        role.text = "Role: " + cData.characterRole.roleName;
        atk.text = cData.LevelAtk + "";
        def.text = cData.characterRole.damageReduction + "%";
        range.text = cData.range + "";
    }
    public void ButtonClick(int buttonSelection)
    {
            switch ((Selection)buttonSelection)
            {
                case Selection.EQUIPMENT:
                    break;
                case Selection.BACK:
                    UIManager.instance.BackToMenu();
                    break;
                default:
                    break;
            }
    }
    public void ButtonClick(Selection buttonSelection)
    {
        switch (buttonSelection)
        {
            case Selection.EQUIPMENT:
                break;
            case Selection.BACK:
                UIManager.instance.BackToMenu();
                break;
            default:
                break;
        }
    }
    public void ButtonHoverEnter(int buttonSelection)
    {
        selectHighlights[buttonSelection].SetActive(true);
    }
    public void ButtonHoverExit(int buttonSelection)
    {
        selectHighlights[buttonSelection].SetActive(false);
    }
}
