using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SkillUIBlock : MonoBehaviour
{
    [SerializeField] Image skillFG;
    [SerializeField] TMP_Text skillText,skillKey;
    [SerializeField] int skillNum;
    [SerializeField] Image bar;
    [SerializeField] Color full, halfway, empty;
    float maxTimer;
    CharacterData leaderData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeBlock()
    {
        leaderData = PartyManager.instance.memberRef[PartyManager.instance.GetLeaderTransform()].characterData;
        maxTimer = leaderData.GetMaxSkilLTime(skillNum);
        skillKey.text = (skillNum + 1) + "";
        if (maxTimer == -1)
        {
            skillText.text = "";
            bar.fillAmount = 0;
        }
        else
        {
            skillText.text = leaderData.GetSkillText(skillNum);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(maxTimer == -1)
        {
            return;
        }
        bar.fillAmount = (maxTimer - leaderData.skillTimer[skillNum]) / maxTimer;
        if(bar.fillAmount >= 1)
        {
            bar.color = full;
        }
        else if (bar.fillAmount >= 0.5)
        {
            bar.color = halfway;
        }
        else
        {
            bar.color = empty;
        }
    }
}
