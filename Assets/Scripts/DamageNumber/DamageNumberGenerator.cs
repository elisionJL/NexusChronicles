using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageNumberGenerator : MonoBehaviour
{
    public static DamageNumberGenerator instance;
    [SerializeField] TMP_FontAsset playerHitFont, playerHitByFont;
    [SerializeField] GameObject damageNumberPrefab;
    [SerializeField] int maxDamageNumbers = 100;
    DamageNumber[] damageNumbers;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        damageNumbers = new DamageNumber[maxDamageNumbers];
        for(int i = 0; i < maxDamageNumbers;++i)
        {
            damageNumbers[i] = Instantiate(damageNumberPrefab, transform).GetComponent<DamageNumber>();
            damageNumbers[i].setPos(i);
            damageNumbers[i].gameObject.SetActive(false);
        }
    }
    public void DisableDamageNumber(int pos)
    {
        damageNumbers[pos].gameObject.SetActive(false);
    }
    public void GeneratePlayerHit(Vector3 position,string damage)
    {
        for(int i =0; i < maxDamageNumbers; ++i)
        {
            if(damageNumbers[i].gameObject.activeSelf == false)
            {
                damageNumbers[i].PlayerHit(position, playerHitFont,damage);
                break;
            }
        }
    }
    public void GeneratePlayerGetsHit(Vector3 position,string damage)
    {
        for (int i = 0; i < maxDamageNumbers; ++i)
        {
            if (damageNumbers[i].gameObject.activeSelf == false)
            {

                damageNumbers[i].CharacterHit(position, playerHitByFont,damage);
                break;
            }
        }
    }
}
