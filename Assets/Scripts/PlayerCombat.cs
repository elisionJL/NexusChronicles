using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this class controls the actions of the player-controlled character in combat
public class PlayerCombat : MonoBehaviour
{
    CombatManager combatManager;
    enum STATES
    {
        IDLING = 0,
        WALKING = 1,
        ATTACKING = 2,
        GUARDING,
        UNLEASH,
        REVIVING,
        DOWNED
    }
    STATES currentState;
    // Start is called before the first frame update
    void Start()
    {
        combatManager = CombatManager.instance;
        currentState = STATES.IDLING;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
