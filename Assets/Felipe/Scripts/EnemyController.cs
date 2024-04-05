using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;


    [Header("Movement")]
    [SerializeField] private float walkingSpeed = 4f;
    [SerializeField] private float runningSpeed = 10f;
    private float _speed;

    public float speed
    {
        get { return _speed; }
        private set
        {
            _speed = value;
            agent.speed = value;
            animator.SetFloat("speed", value);
        }
    }



    [Header("Patroling")]
    [SerializeField] private Vector3 walkPoint;
    bool walkPointSet;
    [SerializeField] private float walkPointRange = 15f;
    [SerializeField] private float walkPivotX = 25f;
    [SerializeField] private float walkPivotZ = 33f;
    [SerializeField] private float walkTimer = 0f;


    [Header("Attacking")]
    [SerializeField] GameObject biteHitBox;
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
            animator.SetBool("isAlive", value);
        }
    }
    [SerializeField] private bool isDamagable = true;
    bool alreadyAttacked;
    [SerializeField] BoxCollider biteBoxCollider;

    [Header("States")]
    [SerializeField] private float sightRange = 20f;
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        biteHitBox = gameObject.transform.Find("BiteHitBox").gameObject;
        biteBoxCollider = biteHitBox.GetComponentInChildren<BoxCollider>();
        biteBoxCollider.enabled = false;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(50);
        }
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (isAlive)
        {
            if (!playerInSightRange && !playerInAttackRange) Patroling();
            else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            else if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
        else speed = 0f;

        // agent.speed = speed;
        // animator.SetFloat("speed", speed);
        // ChangeAnimations();
        if (walkPointSet)
        {
            walkTimer += Time.deltaTime;
            if (walkTimer >= 5f)
            {
                walkPointSet = false;
                walkTimer = 0f;
            }

        }



    }

    // private void ChangeAnimations()
    // {

    // }

    private void Patroling()
    {
        if (!walkPointSet) Invoke(nameof(SearchWalkPoint), timeBetweenAttacks);

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
            speed = 0f;
        }
        else
        {
            speed = walkingSpeed;
        }
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range to walk to
        float randomX = Random.Range(-walkPointRange, walkPointRange) * 2f;
        float randomZ = Random.Range(-walkPointRange, walkPointRange) * 2f;

        walkPoint = new Vector3(walkPivotX + randomX, transform.position.y, walkPivotZ + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;

        walkTimer = 0f;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(playerTransform.position);
        speed = runningSpeed;
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(playerTransform);
        speed = 0f;

        if (!alreadyAttacked)
        {
            // Attack code
            animator.SetTrigger("isAttacking");

            Invoke(nameof(DealDamage), timeToHit);


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void DealDamage()
    {
        StartCoroutine(activateBiteHitBox());
    }

    IEnumerator activateBiteHitBox()
    {
        biteBoxCollider.enabled = true;
        yield return new WaitForSeconds(timeDamage);
        biteBoxCollider.enabled = false;
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
            animator.SetTrigger("isHit");
            isDamagable = false;
            Invoke(nameof(ResetDamagable), 1f);
        }

        if (health <= 0) StartCoroutine(onDeath());
    }

    IEnumerator onDeath()
    {
        isAlive = false;
        isDamagable = false;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject.transform.parent.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player") && !hasHit)
        {
            //Debug.Log("Damage to: " + other.gameObject.name);
            other.GetComponent<PlayerController>().TakeDamage(attackDamage);
            hasHit = true;
        }

    }

    // private void EnemyDeath()
    // {

    //     Destroy(gameObject);
    // }
}
