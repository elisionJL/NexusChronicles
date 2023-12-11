using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderScript : MonoBehaviour
{
    [SerializeField]Transform wanderOrigin;
    [SerializeField] float radius = 5;
    float timer = 0;
    [SerializeField]float timeToChange = 30;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= 0)
        {
            Vector3 origin = wanderOrigin.position;
            float angle = Random.Range(0, 360);
            float radian = Mathf.Deg2Rad * angle;
            Vector3 direction = Vector3.zero;
            direction.x = radius * Mathf.Cos(radian);
            direction.z = radius * Mathf.Sin(radian);
            transform.position = direction + origin;
            timer = timeToChange;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(wanderOrigin.position, radius);
    }
}
