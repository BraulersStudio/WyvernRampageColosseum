using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(playerController.attackDamage);
            Debug.Log("Hit: " + other.gameObject.name);
        }
    }
}
