using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PortraitBoxes : MonoBehaviour
{
    [SerializeField] GameObject highlight;
    [SerializeField] TMP_Text nameText;
    void Start()
    {
        
    }
    public void SetName(string name)
    {
        nameText.text = name;
    }
    public void EnableHightlight()
    {
        highlight.SetActive(true);
    }
    public void DisableHightlight()
    {
        highlight.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
