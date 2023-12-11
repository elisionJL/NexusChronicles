using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animtest : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetMovementInput();
    }
    void GetMovementInput()
    {
        float valueX = Input.GetAxisRaw("Horizontal");
        float valueY = Input.GetAxisRaw("Vertical");
        if (valueX == 0 && valueY == 0)
        {
            animator.SetBool("Moving", false);
            animator.SetFloat("Velocity", 0);
            return;
        }
        else
        {
            animator.SetBool("Moving", true);
            animator.SetFloat("Velocity", 1);
        }
    }
    public void FootR()
    {
        return;
    }
    public void FootL()
    {
        return;
    }
}
