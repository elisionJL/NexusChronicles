using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public enum SKILLNAME
    {
        CRUSH,
        HEAVYSTRIKE,
        BACKSTAB,
        TAUNT,
        DEFENSESTANCE,
        BASH,
        HEAL,
        AREAHEAL,
        CURSE
    }
    [SerializeField] List<Skill> tempSkillList = new List<Skill>();
    public Dictionary<SKILLNAME,Skill> skillList = new Dictionary<SKILLNAME,Skill>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
