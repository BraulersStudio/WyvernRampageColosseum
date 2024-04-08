using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    [Header("Movement")]
    [SerializeField] private float walkingSpeed = 4f;
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float _speed;

    protected float speed
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
    [SerializeField] BoxCollider biteBoxCollider;
    [SerializeField] private float timeBetweenAttacks = 3f;
    [SerializeField] private float timeToHit = 0.8f;
    [SerializeField] private float timeDamage = 0.3f;
    [SerializeField] public int health = 100;
    [SerializeField] private int attackDamage = 20;
    bool hasHit = false;
    private bool _isAlive = true;
    public bool isAlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            animator.SetBool("isAlive", value);
        }
    }
    private bool isDamagable = true;
    bool alreadyAttacked;

    [Header("States")]
    [SerializeField] private float sightRange = 20f;
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

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
        Move();
    }

    #region Movement
    private void Move()
    {
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

        // Check if it takes more than 3 seconds walking, set walkpoint again
        if (walkPointSet)
        {
            walkTimer += Time.deltaTime;
            if (walkTimer >= 3f)
            {
                walkPointSet = false;
                walkTimer = 0f;
            }

        }
    }

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

    #endregion

    #region Attack
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

    #endregion

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !hasHit)
        {
            other.GetComponent<PlayerController>().TakeDamage(attackDamage);
            hasHit = true;

        }
    }
}
