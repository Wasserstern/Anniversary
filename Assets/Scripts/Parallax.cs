using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public float parallaxEffect;
    float  spriteLength;
    Vector3 startPosition;

    SpriteRenderer spriteRenderer;
    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteLength = spriteRenderer.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        float currentDistance = Vector2.Distance((Vector2)startPosition, (Vector2)cam.transform.position);
        currentDistance *= parallaxEffect;
        float newXPosition = startPosition.x + cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

        if(currentDistance >= spriteLength){
            if(cam.transform.position.x > startPosition.x){
                startPosition = new Vector3(startPosition.x + spriteLength, startPosition.y, startPosition.z);
            }
            else{
                startPosition = new Vector3(startPosition.x - spriteLength, startPosition.y, startPosition.z);
            }
        }
    }
}
