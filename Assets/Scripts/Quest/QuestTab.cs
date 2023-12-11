using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuestTab : MonoBehaviour
{
    public int questPos = 0;
    [SerializeField] TMP_Text title;
    public void updateQuestShown()
    {
        QuestManager.instance.ShowQuest(questPos);
    }
    public void SetTitle(string _title)
    {
        title.text = _title;
    }
}
