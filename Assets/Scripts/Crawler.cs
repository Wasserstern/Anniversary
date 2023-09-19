using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonoBehaviour
{

    public float defaultSpeed;
    public bool isFacingRight;

    [SerializeField]
    bool isActive;
    public Transform groundCheckTransform;
    SpriteRenderer renderer;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    void SetCrawler(float speed){
        if(Random.Range(0, 2) == 0){
            isFacingRight = true;
        }
        else{
            isFacingRight = false;
            float distanceToCrawler = Vector2.Distance(groundCheckTransform.position, transform.position);
            groundCheckTransform.localPosition = new Vector2(groundCheckTransform.localPosition.x - distanceToCrawler * 2, groundCheckTransform.localPosition.y);
        }
        defaultSpeed = speed;
        isActive =true;
    }
    void SetCrawlerLeft(float speed){
        
        isFacingRight = false;
        float distanceToCrawler = Vector2.Distance(groundCheckTransform.position, transform.position);
        groundCheckTransform.localPosition = new Vector2(groundCheckTransform.localPosition.x - distanceToCrawler * 2, groundCheckTransform.localPosition.y);
        defaultSpeed = speed;
        isActive =true;
        
    }

    void Update()
    {
        if(isActive){

             CheckGround();

            if(isFacingRight){
                transform.position = new Vector3(transform.position.x + defaultSpeed * Time.deltaTime, transform.position.y, transform.position.z);
                renderer.flipX = true;
            }
            else{
                transform.position = new Vector3(transform.position.x - defaultSpeed * Time.deltaTime, transform.position.y, transform.position.z);
                renderer.flipX = false;
            }
        }
       
    }

    void CheckGround(){

        Vector2 rayDirection = (new Vector2(groundCheckTransform.position.x, groundCheckTransform.position.y -1) - (Vector2)groundCheckTransform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(groundCheckTransform.position, rayDirection, 1f, LayerMask.GetMask("Ground"));
        if (hit.collider == null){
            float distanceToCrawler = Vector2.Distance(groundCheckTransform.position, transform.position);
            if(isFacingRight){
                isFacingRight = false;
                groundCheckTransform.localPosition = new Vector2(groundCheckTransform.localPosition.x - distanceToCrawler * 2, groundCheckTransform.localPosition.y);
            }
            else{
                isFacingRight = true;
                 groundCheckTransform.localPosition = new Vector2(groundCheckTransform.localPosition.x + distanceToCrawler * 2, groundCheckTransform.localPosition.y);
            }
        }
        else{
            // Do nothing just move Crawler
        }
    }
}
