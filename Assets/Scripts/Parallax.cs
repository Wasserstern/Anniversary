using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{   
    
    AllManager allmng;
    
    public Camera cam;
    public GameObject Lara;
    public float parallaxEffect;
    float  spriteLength;
    Vector3 startPosition;

    SpriteRenderer spriteRenderer;
    void Start()
    {
        allmng = GameObject.Find("AllManager").GetComponent<AllManager>();
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteLength = spriteRenderer.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        float camDistance = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPosition.x + camDistance, Lara.transform.position.y + allmng.yCameraOffset, 0f);

        if (temp > startPosition.x + spriteLength){
            startPosition = new Vector3(startPosition.x + spriteLength, Lara.transform.position.y + allmng.yCameraOffset, 0f);
        }
        else if(temp < startPosition.x - spriteLength){
            startPosition = new Vector3(startPosition.x - spriteLength, Lara.transform.position.y + allmng.yCameraOffset, 0f);
        }
    }

    void FixedUpdate(){
        
    }
}
