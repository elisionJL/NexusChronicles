using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class CharacterMenu : MonoBehaviour
{
    [SerializeField] GameObject[] selectHighlights = new GameObject[3];
    [System.Serializable]public enum Selection
    {
        STATS,
        SETTINGS,
        BACK,
        TOTAL
    }
    [SerializeField] Image expFG;
    [SerializeField] TMP_Text levelText, expText,goldText;
    Selection selectIndex = Selection.STATS;
    // Start is called before the first frame update
    void Start()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
        //go down the list
        if (Input.GetKeyUp(KeyCode.S))
        {
            selectHighlights[(int)selectIndex].SetActive(false);
            selectIndex = (Selection)((int)(++selectIndex) % (int)Selection.TOTAL);
            selectHighlights[(int)selectIndex].SetActive(true);
        }
        //go up the list
        if (Input.GetKeyUp(KeyCode.W))
        {
            selectHighlights[(int)selectIndex].SetActive(false);
            selectIndex--;
            if (selectIndex < Selection.STATS)
            {
                selectIndex = Selection.STATS;
            }
            selectHighlights[(int)selectIndex].SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            ButtonClick(selectIndex);
        }
    }
    private void OnEnable()
    {
        selectHighlights[(int)selectIndex].SetActive(false);
        selectIndex = Selection.STATS;
        selectHighlights[(int)selectIndex].SetActive(true);
    }
    public void UpdateExpLevel(int _level, float _currentExp, float _requiredExp)
    {
        levelText.text = "Lvl " + _level;
        expText.text = "Exp: " + _currentExp + "/" + _requiredExp;
        expFG.fillAmount = _currentExp / _requiredExp;
    }
    public void UpdateGold(int _gold)
    {
        goldText.text = "Gold:" + _gold;
    }
    public void ButtonClick(int buttonSelection)
    {
            switch ((Selection)buttonSelection)
            {
            case Selection.STATS:
                UIManager.instance.OpenStatMenu();
                break;
            case Selection.BACK:
                    UIManager.instance.ExitMenu();
                    break;
                default:
                    break;
            }
    }
    public void ButtonClick(Selection buttonSelection)
    {
        switch (buttonSelection)
        {
            case Selection.STATS:
                UIManager.instance.OpenStatMenu();
                break;
            case Selection.BACK:
                UIManager.instance.ExitMenu();
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
