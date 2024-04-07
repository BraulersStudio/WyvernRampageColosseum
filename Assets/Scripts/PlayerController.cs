using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //References    
    private CharacterController controller;
    private Animator anim;

    // Movement
    [Header("Movement")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float walkSpeed, runSpeed, backSpeed, gravity;
    [SerializeField] private LayerMask groudnMask;
    [SerializeField] private Transform groundCheck;
    private Vector3 move;
    private bool isGrounded;
    private Vector3 velocity;
    private bool isWalkingBackwards = false;
    private float groundDistance = 0.4f;
    public string msn;

    [Header("Attacking")]
    [SerializeField] GameObject weaponGameObject;
    [SerializeField] BoxCollider weaponBoxCollider;
    [SerializeField] private float timeBetweenAttacks = 0.5f;
    [SerializeField] private float timeToHit = 0.8f;
    [SerializeField] private float timeDamage = 0.3f;
    [SerializeField] public int health = 100;
    [SerializeField] public int attackDamage = 20;
    private bool isDamagable = true;
    private bool hasHit = false;
    private bool _isAlive = true;
    public bool isAlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            anim.SetBool("isAlive", value);
        }
    }
    bool alreadyAttacked;

    [Header("PowerUp")]
    private bool isPowerUpActive;
    [SerializeField] private float powerUpSpeedMultiplier = 1f;
    [SerializeField] private int powerUpDamageMultiplier = 1;
    [SerializeField] private int powerUpHealth = 40;
    [SerializeField] private float powerUpTime = 5f; // Time of power up active in seconds
    private float powerUpTimer = 0f;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        weaponGameObject = GameObject.Find("weaponHitBox").gameObject;
        weaponBoxCollider = weaponGameObject.GetComponentInChildren<BoxCollider>();
        weaponBoxCollider.enabled = false;

    }

    private void Update()
    {
        Move();
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }

        if (isPowerUpActive)
        {
            if (powerUpTimer < powerUpTime)
            {
                powerUpTimer += Time.deltaTime;
            }
            else
            {
                ResetPowerUp();
            }
        }
    }

    #region Movement

    private void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groudnMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (z < 0) isWalkingBackwards = true;
        else isWalkingBackwards = false;

        move = transform.right * x + transform.forward * z;
        move = move.normalized;

        // Verifies if the player is moving and if the player is pressing shift
        // to change the speed accordingly
        if (move != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            Walk();
        }
        else if (move != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        else if (move == Vector3.zero)
        {
            Idle();
        }

        controller.Move(move * speed * powerUpSpeedMultiplier * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Idle()
    {
        anim.SetFloat("speed", 0f);
    }
    private void Walk()
    {
        if (isWalkingBackwards)
        {
            speed = backSpeed;
            anim.SetFloat("speed", -backSpeed);
        }
        else
        {
            speed = walkSpeed;
            anim.SetFloat("speed", walkSpeed);
        }

    }
    private void Run()
    {
        if (isWalkingBackwards)
        {
            speed = backSpeed;
            anim.SetFloat("speed", -backSpeed);
        }
        else
        {
            speed = runSpeed;
            anim.SetFloat("speed", runSpeed);
        }
    }

    #endregion


    #region Attack
    private void Attack()
    {
        speed = 0f;

        if (!alreadyAttacked)
        {
            // Attack code
            anim.SetTrigger("isAttacking");

            Invoke(nameof(DealDamage), timeToHit);


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void DealDamage()
    {
        StartCoroutine(activateWeaponHitBox());
    }

    IEnumerator activateWeaponHitBox()
    {
        weaponBoxCollider.enabled = true;
        yield return new WaitForSeconds(timeDamage);
        weaponBoxCollider.enabled = false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        hasHit = false;
    }

    #endregion


    #region TakeDamage

    private void ResetDamagable()
    {
        isDamagable = true;
    }

    public void TakeDamage(int damage)
    {
        if (isDamagable)
        {
            speed = 0f;
            health -= damage;
            msn = "Player hit";
            anim.SetTrigger("isHit");
            isDamagable = false;
            Invoke(nameof(ResetDamagable), 1f);
        }

        if (health <= 0) StartCoroutine(onDeath());
    }

    IEnumerator onDeath()
    {

        isAlive = false;
        isDamagable = false;
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    #endregion

    private void ResetPowerUp()
    {
        isPowerUpActive = false;
        powerUpTimer = 0f;
        powerUpSpeedMultiplier = 1f;
        powerUpDamageMultiplier = 1;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && !hasHit)
        {
            msn = "Damage to: " + other.gameObject.name;
            other.GetComponent<EnemyController>().TakeDamage(attackDamage * powerUpDamageMultiplier);
            hasHit = true;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Heart"))
        {
            msn = "Pickup Heart";
            health += powerUpHealth;
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Speed"))
        {
            msn = "Pickup Speed";
            powerUpSpeedMultiplier = 1.5f;
            isPowerUpActive = true;
            other.gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("Damage"))
        {
            msn = "Pickup Damage";
            powerUpDamageMultiplier = 2;
            isPowerUpActive = true;
            other.gameObject.SetActive(false);
        }
    }
}