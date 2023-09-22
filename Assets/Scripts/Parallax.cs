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
    float  length;
    float startPosition;

    public float yOffset;

    void Start()
    {
        allmng = GameObject.Find("AllManager").GetComponent<AllManager>();
        startPosition = transform.position.x;

        length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate(){
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        float camDistance = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPosition + camDistance, Lara.transform.position.y + allmng.yCameraOffset + yOffset, 0f);

        if (temp > startPosition + length){
            startPosition += length;
        }
        else if(temp < startPosition - length){
            startPosition -= length;
        }
    }

    void FixedUpdate(){
        
    }
}
