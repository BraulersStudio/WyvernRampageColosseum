using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{

    public PlayerController playerController;


    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<EnemyController>().TakeDamage(playerController.attackDamage);
        Debug.Log("Hit: " + other.gameObject.name);
    }
}
