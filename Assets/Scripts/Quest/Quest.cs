using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{
    [System.Serializable]
    public class KillObjective
    {
        public int amount;
        public EnemyType.ENEMY KillTarget;
    }
    [TextArea]
    public string title, description,objectiveText;
    [SerializeField] KillObjective[] objectives;
    [SerializeField] int gold, exp;
    public bool completeOnRequirementMet= false;
    // Start is called before the first frame update
    void Start()
    {
    }
    public string GetRewardText()
    {
        return "exp: " + exp +"\n" + "gold: " + gold;
    }

    public KillObjective[] GetObjective()
    {
        return objectives;
    }
    public string GetObjectiveText()
    {
        return objectiveText;
    }
    public int GetNumberOfObjectives()
    {
        return objectives.Length;
    }
}
