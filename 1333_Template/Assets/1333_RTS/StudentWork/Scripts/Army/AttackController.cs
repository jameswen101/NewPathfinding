using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform targetToAttack;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack == null)
        {
            // If the collider is an enemy, set it as the target to attack
            targetToAttack = other.transform;
            Debug.Log("Target acquired: " + targetToAttack.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack == other.transform)
        {
            // If the collider is the current target, clear the target
            Debug.Log("Target lost: " + targetToAttack.name);
            targetToAttack = null;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
