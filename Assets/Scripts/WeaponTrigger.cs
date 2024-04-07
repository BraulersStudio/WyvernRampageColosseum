using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    public PlayerController playerController;
    AudioSource audioSource;
    public AudioClip sndwp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(playerController.attackDamage);
            Debug.Log("Hit: " + other.gameObject.name);
            audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(sndwp);
        }
    }
}
