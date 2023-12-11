using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DotProductTest : MonoBehaviour
{
    public TMP_Text text;
    public Transform enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos1, pos2;
        pos1 = enemy.position;
        pos1.y = 0;
        pos2 = transform.position;
        pos2.y = 0;
        Vector3 direction = (pos1 - pos2).normalized;    
        direction.y = 0;
        Vector3 playerForward = enemy.forward;
        playerForward.y = 0;
        text.text = Vector3.Dot(playerForward, direction) + "";
    }
}
