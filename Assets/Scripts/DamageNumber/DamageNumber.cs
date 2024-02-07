using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageNumber : MonoBehaviour
{
    //the position of the damage number in the generator array
    int pos = 0;
    [SerializeField]TMP_Text text;
    float timer = 0;
    Vector3 change;
    private void Awake()
    {
        //text = GetComponent<TMP_Text>();
        change = new Vector3(0, 1, 0);
    }
    public void setPos(int _pos)
    {
        pos = _pos;
    }
    public void PlayerHit(Vector3 pos,TMP_FontAsset font, string damage)
    {
        text.font = font;
        transform.position = pos + new Vector3(Random.Range(-1,1f), Random.Range(0, 1f), Random.Range(-1, 1f));
        text.text = damage;
        gameObject.SetActive(true);
        timer = 2;
    }
    public void CharacterHit(Vector3 pos, TMP_FontAsset font,string damage)
    {
        text.font = font;
        transform.position = pos + new Vector3(Random.Range(-1, 1f), Random.Range(0, 1f), Random.Range(-1, 1f));
        text.text = damage;
        gameObject.SetActive(true);
        timer = 2;
    }
    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            change.y = Time.deltaTime;
            transform.Translate(change);
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
