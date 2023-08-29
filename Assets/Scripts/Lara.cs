using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lara : MonoBehaviour
{

    AllManager allmng;
    Rigidbody2D rgbd;
    Collider2D col;

    public float normalSpeed;
    public float turboSpeed;
    public float acceleration;
    public float turboAcceleration;
    public int health;

    public float jumpForce;
    public float maxJumpTime;
    public float fallTime;

    public float minGravity;
    public float maxGravity;
    public float normalGravity;

    public float groundCheckXSize;
    public float groundCheckYSize;
    public float groundCheckDistance;
    [SerializeField]
    float currentSpeed;
    [SerializeField]
    float currentGravity;

    [SerializeField]
    bool isPressingJump;
    [SerializeField]
    bool isGrounded;
    [SerializeField]
    bool isTurbo;
    [SerializeField]
    bool isJumping;

    IEnumerator jumpCoroutine;
    
    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        jumpCoroutine = Jump();
    }

    void Update()
    {
        
        GetInput();
        CheckGround();

        if(isGrounded && isPressingJump && rgbd.velocity.y <= 0 && !isJumping){
            isJumping = true;
            jumpCoroutine = Jump();
            StartCoroutine(jumpCoroutine);
        }
        currentSpeed = rgbd.velocity.x;
        float nextNormalSpeed = currentSpeed + acceleration * Time.deltaTime;
        float nextTurboSpeed = currentSpeed + turboAcceleration * Time.deltaTime;

        if(isTurbo){
            if(nextTurboSpeed > turboSpeed){
                nextTurboSpeed = turboSpeed;
            }
            rgbd.velocity = new Vector2(nextTurboSpeed, rgbd.velocity.y);
        }
        else{
            if(nextNormalSpeed > normalSpeed){
                nextNormalSpeed = normalSpeed;
            }
            rgbd.velocity = new Vector2(nextNormalSpeed, rgbd.velocity.y);
        }
    }

    void GetInput(){
        isPressingJump = Input.GetKey(KeyCode.Space);
    }

    void CheckGround(){

        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y);
        Vector2 size = new Vector2(groundCheckXSize, groundCheckYSize);
        Vector2 direction = (new Vector2(origin.x, origin.y -1) - origin).normalized;
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, direction, groundCheckDistance, LayerMask.GetMask("Ground"));
        if(hit.collider != null){
            if(rgbd.velocity.y <= 0 && !isJumping){
                isGrounded = true;
                StopCoroutine(jumpCoroutine);
                rgbd.gravityScale = normalGravity;
            }
            else{
                isGrounded = false;
            }
        }
        else{
            isGrounded = false;
        }
    }

    IEnumerator Jump(){
        float startTime = Time.time;
        float elapsedTime = 0f;
        rgbd.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        rgbd.gravityScale = minGravity;
        while(isPressingJump && Time.time - startTime < maxJumpTime){
            float t = elapsedTime / maxJumpTime;
            rgbd.gravityScale = Mathf.Lerp(minGravity, normalGravity, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isJumping = false;

        startTime = Time.time;
        elapsedTime = 0f;
        float currentGravityScale = rgbd.gravityScale;
        while(Time.time - startTime < fallTime){
            float t  = elapsedTime / fallTime;
            rgbd.gravityScale = Mathf.Lerp(currentGravityScale, maxGravity, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
