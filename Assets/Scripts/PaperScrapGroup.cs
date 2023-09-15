using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperScrapGroup : MonoBehaviour
{
    public GameObject PaperScrap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateWithPattern(int patternIndex, float groundXScale){
        switch(patternIndex){
            case 0:{
                // Line pattern
                for(int i = 0; i < Mathf.FloorToInt(groundXScale); i++){
                    GameObject nextPaperScrap = GameObject.Instantiate(PaperScrap, transform);
                    nextPaperScrap.transform.localPosition = new Vector2(i, nextPaperScrap.transform.localPosition.y);
                }
                break;
            }
            case 1:{
                // Circle pattern
                break;
            }
        }
    }
}
