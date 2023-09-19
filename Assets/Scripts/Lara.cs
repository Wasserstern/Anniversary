using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lara : MonoBehaviour
{

    public AudioClip jump;
    public AudioClip hit;
    AudioSource audioSource;
    Animator animator;
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

    public float retryTime;
    public float gameStartTime;
    public float deathHitForce;

    IEnumerator jumpCoroutine;
    public bool gameStarted;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameObject.Find("AppearCircle").GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(GameStart());
        animator = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        jumpCoroutine = Jump();
    }

    void Update()
    {
        if(gameStarted){
            GetInput();
            CheckGround();

            if(isGrounded && isPressingJump && rgbd.velocity.y <= 0 && !isJumping){
                isJumping = true;
                isGrounded = false;
                animator.SetBool("isGrounded", isGrounded);
                animator.SetTrigger("jump");
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
            animator.SetBool("isJumping", isJumping);
        }
        
    }

    void GetInput(){
        isPressingJump = Input.GetKey(KeyCode.Space);

        if(Input.GetKeyDown(KeyCode.R)){
            StartCoroutine(Retry());
        }
    }

    IEnumerator Retry(){
        GameObject disappearCircle = GameObject.Find("DisappearCircle");
        float startTime = Time.time;
        float elapsedTime = 0f;
        Vector3 startScale = disappearCircle.transform.localScale;
        while(Time.time - startTime < retryTime){
            float f = elapsedTime / retryTime;
            f = EaseFunctions.easeInOutQuint(f);
            disappearCircle.transform.localScale = Vector3.Lerp(startScale, new Vector3(3, 3, 3), f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        disappearCircle.transform.localScale = new Vector3(3, 3, 3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    IEnumerator GameStart(){
        GameObject appearCircle = GameObject.Find("AppearCircle");
        float startTime = Time.time;
        float elapsedTime = 0f;
        Vector3 startScale = appearCircle.transform.localScale;
        while(Time.time - startTime < gameStartTime){
            float f = elapsedTime / gameStartTime;
            f = EaseFunctions.easeInOutQuint(f);
            appearCircle.transform.localScale = Vector3.Lerp(startScale, new Vector3(80, 80, 80), f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        appearCircle.transform.localScale = new Vector3(80, 80, 80);
        GameObject.Destroy(appearCircle);
        animator.SetTrigger("GameStart");
        gameStarted = true;
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
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")){
                animator.SetTrigger("fall");
            }
        }
        animator.SetBool("isGrounded", isGrounded);
    }

    IEnumerator Jump(){
        audioSource.clip = jump;
        audioSource.Play();
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

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            // Boost upwards
            Debug.Log("Should boost");
            jumpCoroutine = Jump();
            StartCoroutine(Jump());
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Ocean")){
            gameStarted = false;
            animator.SetTrigger("death");
            rgbd.velocity = new Vector3(0, 0, 0);
            rgbd.AddForce(transform.up * deathHitForce, ForceMode2D.Impulse);
            rgbd.gravityScale = 0.5f;
            StartCoroutine(Retry());
        }
    }
}
