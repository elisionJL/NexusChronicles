using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    [SerializeField] Image hpForeground;
    [SerializeField] TMP_Text nameText,hpText;
    public CharacterData stats;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void SetStats(CharacterData _stats)
    {
        stats = _stats;
        nameText.text = stats.transform.gameObject.name;

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        hpForeground.fillAmount =  stats.hp / stats.levelHp;
        hpText.text = Mathf.FloorToInt( stats.hp) + "";
    }
}
