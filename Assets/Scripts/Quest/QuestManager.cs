using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    [System.Serializable]
    public class TrackObjective
    {
        
        public Quest quest;
        //each quest has a set of Objectives which we turn into targets
        public Dictionary<EnemyType.ENEMY, int> requirements = new Dictionary<EnemyType.ENEMY, int>();
        public Dictionary<EnemyType.ENEMY, int> progress = new Dictionary<EnemyType.ENEMY, int>();
        public EnemyType.ENEMY[] TrackedEnemies;
        public bool TurnInPointMet = false;
        //constructor 
        public TrackObjective(Quest _quest)
        {
            quest = _quest;
            //get the quest objectives
            Quest.KillObjective[] currentTargets = quest.GetObjective();
            //if true than there is not turn in point thus this condition is met.
            TurnInPointMet = quest.completeOnRequirementMet;
            TrackedEnemies = new EnemyType.ENEMY[ quest.GetNumberOfObjectives()];
            //add each of them into the requirements and create a progress for each objective
            for (int i = 0; i < currentTargets.Length; ++i)
            {
                Quest.KillObjective objective = currentTargets[i];
                requirements.Add(objective.KillTarget, objective.amount);
                progress.Add(objective.KillTarget, 0);
                TrackedEnemies[i] = objective.KillTarget;
                
            }
        }
    }
    [SerializeField] TMP_Text title, description, objective, reward;
    [SerializeField] RectTransform FirstOrigin, SecondOrigin;
    [SerializeField] GameObject tabPrefab;
    List<TrackObjective> acceptedQuest = new List<TrackObjective>();
    List<Quest> completedQuests = new List<Quest>();
    [SerializeField] Quest Test;
    [SerializeField] Quest Test2;
    Vector3 positionSpacing;
    [SerializeField]GameObject questAccept, questComplete;
    [SerializeField] TMP_Text acceptTitle, completeButtonTitle;
    [SerializeField] Button completeButton;
    //when asked to accept or complete what quest is being aske
    Quest currentPromptedQuest;
    List<QuestTab> questTabs = new List<QuestTab>();
   
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            positionSpacing = SecondOrigin.position - FirstOrigin.position;
            gameObject.SetActive(false);
            //Add(Test);
            //Add(Test2);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnEnable()
    {
        if(acceptedQuest.Count == 0)
        {
            title.text = "";
            description.text = "";
            reward.text = "";
            objective.text = "";
            return;
        }
        Quest quest = acceptedQuest[0].quest;
        title.text = quest.title;
        description.text = quest.description;
        reward.text = quest.GetRewardText();
        string objectiveText = quest.GetObjectiveText();
        for (int i = 0; i < quest.GetNumberOfObjectives(); ++i)
        {
            string replaceText = "num" + i;
            objectiveText = objectiveText.Replace(replaceText,""+ acceptedQuest[0].progress[acceptedQuest[0].TrackedEnemies[i]]);
        }
        objective.text = objectiveText;
    }
    public void Add(Quest addedQuest)
    {
        acceptedQuest.Add(new TrackObjective(addedQuest));
      //instatiate the prefab as child of the main panel and get the QuestTabScript
        QuestTab tab = Instantiate(tabPrefab, transform).GetComponent<QuestTab>();
        //add the script to the list
        questTabs.Add(tab);
        //set the position that it would be in
        tab.questPos = questTabs.Count - 1;
        //move the transform to its set location
        tab.transform.position = FirstOrigin.transform.position + (positionSpacing * tab.questPos);
        tab.SetTitle(addedQuest.title);
    }
    public void UpdateQuestrogress(EnemyType.ENEMY target,int amount)
    {
        //go through each quest
        for (int i = 0; i< acceptedQuest.Count;++i)
        {
            //check if the quest requires to kill the enemy
            if (acceptedQuest[i].progress.ContainsKey(target)) { 
                //check if it has been completed already
                if(acceptedQuest[i].progress[target] != acceptedQuest[i].requirements[target])
                {
                    //if not completed add to progress
                    acceptedQuest[i].progress[target] += amount;
                    //check if the quest is suppose to immediately complete or not
                    if (!acceptedQuest[i].quest.completeOnRequirementMet)
                        continue;
                    if (CheckCompleted(acceptedQuest[i].quest))
                    {
                        //add the quest to completed
                        completedQuests.Add(acceptedQuest[i].quest);
                        //add in the rewards here
                        //
                        //
                        //

                        //remove the quest from accepted
                        Remove(acceptedQuest[i].quest);
                        break;
                    }
                }
            }                
        }
    }
    public void UpdateQuestProgress()
    {
        //go through each quest
        for (int i = 0; i < acceptedQuest.Count; ++i)
        {
        }
    }
    public bool CheckCompleted(Quest quest)
    {
        for (int i = 0; i < acceptedQuest.Count; ++i)
        {
            if(acceptedQuest[i].quest ==  quest)
            {
                if (acceptedQuest[i].TurnInPointMet == false)
                {
                    return false;
                }
                for (int check = 0; check < acceptedQuest[i].progress.Count;++check)
                {
                    //check if the progress reaches the requiremnts
                    if (acceptedQuest[i].progress[acceptedQuest[i].TrackedEnemies[check]] != acceptedQuest[i].requirements[acceptedQuest[i].TrackedEnemies[check]])
                    {
                        //if not return not completed
                        return false;
                    }
                }
                break;
            }
            //reaching this means that the quest was never accepted or added
            if(i == acceptedQuest.Count - 1)
            {
                return false;
            }
        }
        //return completed
        return true;
    }
    //called when interacting with the turn in NPC;
    public bool TurnInCheck(Quest quest)
    {
        for (int i = 0; i < acceptedQuest.Count; ++i)
        {
            if (acceptedQuest[i].quest == quest)
            {
                if (acceptedQuest[i].TurnInPointMet)
                {
                    return true;
                }
                for (int check = 0; check < acceptedQuest[i].progress.Count; ++check)
                {
                    //check if the progress reaches the requiremnts
                    if (acceptedQuest[i].progress[acceptedQuest[i].TrackedEnemies[check]] != acceptedQuest[i].requirements[acceptedQuest[i].TrackedEnemies[check]])
                    {
                        //if not return not completed
                        return false;
                    }
                    
                }
                //if all the conditions are met turn it point = true;
                acceptedQuest[i].TurnInPointMet = true;
                return true;
            }
            //reaching this means that the quest was never accepted or added
            if (i == acceptedQuest.Count - 1)
            {
                return false;
            }
        }
        //return completed
        return true;
    }
    public void Remove(Quest quest)
    {
        for (int i = 0; i < questTabs.Count; ++i)
        {
           if (acceptedQuest[i].quest == quest)
            {
                //destroy the prefab
                Destroy(questTabs[i].gameObject);
                //remove from list
                questTabs.RemoveAt(i);
                questTabs.TrimExcess();
                //remove the quest from accepted quests
                acceptedQuest.RemoveAt(i);
                acceptedQuest.TrimExcess();
                //reorder the order of the quests 
                ReOrderQuestTabs();
                break;
            }
        }
    }
    void ReOrderQuestTabs()
    {
        for (int i = 0; i < questTabs.Count; ++i)
        {
            questTabs[i].questPos = i;
            questTabs[i].transform.position = FirstOrigin.transform.position + (positionSpacing * questTabs[i].questPos);
       
        }
        if (questTabs.Count != 0)
        {
            ShowQuest(0);
        }
    }
    public void CompleteQuest(Quest quest)
    {
        //add the quest to completed
        completedQuests.Add(quest);

        //add in the rewards here
        //
        //
        //

        //remove the quest from accepted
        Remove(quest);
    }
    public void OpenQuestAccept(Quest quest)
    {
        currentPromptedQuest = quest;
        questAccept.SetActive(true);
        acceptTitle.text = "Do you want to accept the quest " + '"' + currentPromptedQuest.title + '"' + "?";

    }
    public void OpenQuestComplete(Quest quest)
    {
        if(acceptedQuest.Count == 0)
        {
            CloseQuestComplete();
        }
        currentPromptedQuest = quest;
        gameObject.SetActive(true);
        ShowQuest(0);
        //completeTitle.text = "Turn In " +'"'+ currentPromptedQuest.title +'"'+ "?";
    }
    public void TurnInMet(Quest quest)
    {
        for (int i = 0; i < acceptedQuest.Count; ++i)
        {
            if (acceptedQuest[i].quest == quest)
            {
                acceptedQuest[i].TurnInPointMet = true;
            }
        }
    }
    public void Accept()
    {
        Add(currentPromptedQuest);
    }
    public void TurnIn()
    {
        CompleteQuest(currentPromptedQuest);
    }
    public void CloseQuestAccept()
    {
        questAccept.SetActive(false);
        UIManager.instance.menuIsOpen = false;
    }
    public void CloseQuestComplete()
    {
        gameObject.SetActive(false);
        UIManager.instance.menuIsOpen = false;
    }
    public void ShowQuest(int index)
    {
        Quest quest = acceptedQuest[index].quest;
        if (CheckCompleted(quest))
        {
            completeButtonTitle.text = "Turn In";
            completeButton.interactable = true;
        }
        else
        {
            completeButtonTitle.text = "Not Completed";
            completeButton.interactable = false;
        }
        title.text = quest.title;
        description.text = quest.description;
        reward.text = quest.GetRewardText();
        string objectiveText = quest.GetObjectiveText();
        for (int i = 0; i < quest.GetNumberOfObjectives(); ++i)
        {
            string replaceText = "num" + i;
            objectiveText = objectiveText.Replace(replaceText, "" + acceptedQuest[index].progress[acceptedQuest[index].TrackedEnemies[i]]);
        }
        objective.text = objectiveText;
    }
}
