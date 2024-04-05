using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Variables
    [SerializeField] private float moveSpeed, walkSpeed, runSpeed, backSpeed, gravity, jumpForce;
    [SerializeField] private bool isGrounded;
    private Vector3 moveDirection, velocity; 
    //References    
    private CharacterController controller;
    private Animator anim;
    private Rigidbody playerRb;

    private void Start(){
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        playerRb = GetComponent<Rigidbody>();
    } 

    private void Update(){
        Move();
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void Move(){
        

        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveX, 0, moveZ);
        moveDirection.Normalize(); 
        
        if(isGrounded && velocity.y<0){
            velocity.y = -2f;
        }
        if(isGrounded){
            if(moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift)){
                Walk();
            }
            else if(moveDirection!= Vector3.zero && Input.GetKey(KeyCode.LeftShift)){
                Run();
            }
            else if(moveDirection == Vector3.zero){
                Idle();
            }
            
            moveDirection *= moveSpeed;

            if(Input.GetKey(KeyCode.Space)){
                Jump();
            }
        }
        

        
        controller.Move(moveDirection * Time.deltaTime);       
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
    }

    private void Idle(){
        anim.SetFloat("Speed", 0);
    }

    private void Walk(){
        if(moveDirection.z<0){
            moveSpeed = backSpeed;
            anim.SetFloat("Speed", 0.5f);
        }else{
          moveSpeed = walkSpeed;  
          anim.SetFloat("Speed", 0.1f);
        }
        
    }

    private void Run(){
        if(moveDirection.z<0){
            moveSpeed = backSpeed;
            anim.SetFloat("Speed", 0.5f);
        }else{
          moveSpeed = runSpeed; 
          anim.SetFloat("Speed", 0.2f); 
        }    
    }

    private void Jump(){
       velocity.y = Mathf.Sqrt(jumpForce* -2 *gravity);
       isGrounded=false;
       anim.SetFloat("Speed", 1);
    }
    private void Attack(){
        
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Ground")){
            isGrounded = true;
        } 
    }
}