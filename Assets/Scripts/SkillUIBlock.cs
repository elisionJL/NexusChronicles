using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SkillUIBlock : MonoBehaviour
{
    [SerializeField] Image skillFG;
    [SerializeField] TMP_Text skillText;
    [SerializeField] int skillNum;
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
        skillText.text = leaderData.GetSkillText(skillNum);
    }

    // Update is called once per frame
    void Update()
    {
        skillFG.fillAmount = (maxTimer - leaderData.skillTimer[skillNum]) / maxTimer;
    }
}
