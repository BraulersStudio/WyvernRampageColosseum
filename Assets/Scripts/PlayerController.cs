using System.Collections;
using System.Collections.Generic;
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
    private Rigidbody playerRb;

    public Vector3 move;


    //





    public float speed = 12f;

    public float jumpHeight = 3f;
    public float scale = 2;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groudnMask;




    //


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void Move0()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveX, 0, moveZ);
        moveDirection.Normalize();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (isGrounded)
        {
            if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
            {
                Walk();
            }
            else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
            {
                Run();
            }
            else if (moveDirection == Vector3.zero)
            {
                Idle();
            }

            moveDirection *= moveSpeed;

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }



        controller.Move(moveDirection * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groudnMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

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

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Idle()
    {
        anim.SetFloat("Speed", 0);
    }

    private void Walk()
    {
        if (move.z < 0)
        {
            moveSpeed = backSpeed;
            anim.SetFloat("Speed", 0.5f);
        }
        else
        {
            moveSpeed = walkSpeed;
            anim.SetFloat("Speed", 0.1f);
        }

    }

    private void Run()
    {
        if (move.z < 0)
        {
            moveSpeed = backSpeed;
            anim.SetFloat("Speed", 0.5f);
        }
        else
        {
            moveSpeed = runSpeed;
            anim.SetFloat("Speed", 0.2f);
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
        isGrounded = false;
        anim.SetFloat("Speed", 1);
    }
    private void Attack()
    {
        Debug.Log("Attacking");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}