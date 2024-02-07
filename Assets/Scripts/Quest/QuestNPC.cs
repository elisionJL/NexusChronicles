using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : MonoBehaviour
{
    QuestManager questManager;
    [SerializeField]Quest[] givenQuest;
    byte entered = 0;
    bool taken = false;
    bool completed = false;
    [SerializeField]bool TurnIn = false;
    Dictionary<Quest, bool> questStatus = new Dictionary<Quest, bool>();
    GameObject availableQuestIcon, completeQuestIcon;
    Quest currentQuest = null;
    // Start is called before the first frame update
    void Start()
    {
        questManager = QuestManager.instance;
        for(int i = 0; i < givenQuest.Length; ++i)
        {
            questStatus.Add(givenQuest[i], false);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!TurnIn)
        {
            if (Input.GetKeyDown(KeyCode.E) && entered > 0 && !UIManager.instance.menuIsOpen)
            {
                currentQuest  = GetIndexQuest();
                if (currentQuest == null)
                {
                    return;
                }
                UIManager.instance.menuIsOpen = true;
                questManager.OpenQuestAccept(currentQuest, this);
            }
        }
        else {

            if (Input.GetKeyDown(KeyCode.E) && entered > 0 && !UIManager.instance.menuIsOpen)
            {
                for (int i = 0; i < givenQuest.Length; ++i)
                {
                    questManager.TurnInCheck(givenQuest[i]);
                }
                UIManager.instance.menuIsOpen = true;
                questManager.OpenQuestComplete(GetIndexQuest());
            }
        }
    }
    public void OnQuestAccepted()
    {
        questStatus[currentQuest] = true;
        currentQuest = null;
    }
    void HideIcon()
    {

    }
    Quest GetIndexQuest()
    {
        for(int i =0; i < questStatus.Count; ++i)
        {
            if (questStatus[givenQuest[i]] == false)
            {
                return givenQuest[i];
            }
        }
        return null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractionRange"))
        {
            ++entered;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InteractionRange"))
        {
            --entered;
        }
    }
}
