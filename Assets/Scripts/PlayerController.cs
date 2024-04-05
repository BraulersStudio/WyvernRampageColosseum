using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Variables
    [SerializeField] private float moveSpeed, walkSpeed, runSpeed, backSpeed, gravity, jumpForce;
    [SerializeField] private bool isGrounded;
    public Vector3 moveDirection, velocity;

    //References    
    private CharacterController controller;
    private Animator anim;

    public Vector3 move;

    private bool isWalkingBackwards = false;


    //





    public float speed = 12f;

    public float jumpHeight = 3f;
    public float scale = 2;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groudnMask;




    //


    [Header("Attacking")]
    [SerializeField] GameObject weaponGameObject;
    [SerializeField] BoxCollider weaponBoxCollider;
    [SerializeField] private float timeBetweenAttacks = 3f;
    [SerializeField] private float timeToHit = 0.8f;
    [SerializeField] private float timeDamage = 0.3f;
    [SerializeField] private int health = 100;
    [SerializeField] private int attackDamage = 20;
    bool hasHit = false;
    private bool _isAlive = true;
    [SerializeField]
    public bool isAlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            anim.SetBool("isAlive", value);
        }
    }
    [SerializeField] private bool isDamagable = true;
    bool alreadyAttacked;



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

        if (Input.GetButtonDown("Jump"))
        {
            TakeDamage(50);
        }
    }

    #region Movement
    // private void Move0()
    // {
    //     float moveZ = Input.GetAxis("Vertical");
    //     float moveX = Input.GetAxis("Horizontal");
    //     moveDirection = new Vector3(moveX, 0, moveZ);
    //     moveDirection.Normalize();

    //     if (isGrounded && velocity.y < 0)
    //     {
    //         velocity.y = -2f;
    //     }
    //     if (isGrounded)
    //     {
    //         if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
    //         {
    //             Walk();
    //         }
    //         else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
    //         {
    //             Run();
    //         }
    //         else if (moveDirection == Vector3.zero)
    //         {
    //             Idle();
    //         }

    //         moveDirection *= moveSpeed;

    //         if (Input.GetKey(KeyCode.Space))
    //         {
    //             Jump();
    //         }
    //     }



    //     controller.Move(moveDirection * Time.deltaTime);
    //     velocity.y += gravity * Time.deltaTime;
    //     controller.Move(velocity * Time.deltaTime);

    // }

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

        // if (Input.GetButtonDown("Jump") && isGrounded)
        // {
        //     velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        // }


        controller.Move(move * speed * Time.deltaTime);

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
            Debug.Log("Player hit");
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



    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy") && !hasHit)
        {
            Debug.Log("Damage to: " + other.gameObject.name);
            other.GetComponent<EnemyController>().TakeDamage(attackDamage);
            hasHit = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }


}