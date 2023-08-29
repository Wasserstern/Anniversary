using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{

    public GameObject StandardLevelPart;

    public enum LevelMode { flat, cascade, downhill, uphill, crazy}
    public int maxObjects;
    public float difficulty;
    public float generationIntervalInSeconds;
    public float levelModeIntervalInSeconds;
    public Queue<GameObject> previousObjects;
    public LevelMode currentMode;

    public float minHeight;
    public float maxHeight;
    public float maxDistance;
    
    public float minDistance;

    
    public GameObject lastObject;
    [SerializeField]


    public float cascadeMaxWidth;
    public float cascadeMinWidth;

    public float downHillMinHeight;
    public float downHillMaxHeight;
    public float downHillMinWidth;
    public float downHillMaxWidth;
    
    public float upHillMaxHeight;
    public float upHillMinHeight;
    void Start()
    {
        previousObjects = new Queue<GameObject>();
        StartCoroutine(GenerateTimer());
        StartCoroutine(LevelModeTimer());
    }

    void Update()
    {
        
    }

    IEnumerator GenerateTimer(){
        yield return new WaitForSeconds(generationIntervalInSeconds);
        Generate();
    }

    IEnumerator LevelModeTimer(){
        yield return new WaitForSeconds(levelModeIntervalInSeconds);
        int enumLength = Enum.GetNames(typeof(LevelMode)).Length;
        int[] nextLevelModeChoices = new int[enumLength -1];
        int index = 0;
        for(int i = 0; i < enumLength; i++){
            if(i != (int)currentMode){
                nextLevelModeChoices[index] = i;
                index++;
            }
        }
        currentMode = (LevelMode)nextLevelModeChoices[UnityEngine.Random.Range(0, nextLevelModeChoices.Length)];
        StartCoroutine(LevelModeTimer());
    }

    void Generate(){
        Vector2 nextSpawnPosition = new Vector2();
        Transform lastObjectGround = lastObject.transform.Find("Ground");
        float nextXScale = 1f;
        switch(currentMode){
            case LevelMode.flat:{
                nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectGround.localScale.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y);
                nextXScale = Random.Range(cascadeMinWidth, cascadeMaxWidth);
                break;
            }
            case LevelMode.cascade:{


                Debug.Log(lastObjectGround.localScale.x / 2);
                if(UnityEngine.Random.Range(0, 2) == 0){
                    // Spawn next object above current height
                    nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectGround.localScale.x + UnityEngine.Random.Range(minDistance, maxDistance), lastObject.transform.position.y + Random.Range(0, maxHeight));
                }
                else{
                    // Spawn next object below current height
                    nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectGround.localScale.x  + UnityEngine.Random.Range(minDistance, maxDistance), lastObject.transform.position.y - Random.Range(0, minHeight));
                }
                nextXScale = UnityEngine.Random.Range(cascadeMinWidth, cascadeMaxWidth);
                

                break;
            }
            case LevelMode.downhill:{
                nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectGround.localScale.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y - Random.Range(downHillMinHeight, downHillMaxHeight));
                nextXScale = Random.Range(downHillMinWidth, downHillMaxWidth);
                break;
            }
            case LevelMode.uphill:{
                nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectGround.localScale.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y + Random.Range(upHillMinHeight, upHillMaxHeight));
                nextXScale = Random.Range(downHillMinWidth, downHillMaxWidth);
                break;
            }
            case LevelMode.crazy:{
                 nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectGround.localScale.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y + Random.Range(upHillMinHeight, upHillMaxHeight));
                nextXScale = Random.Range(downHillMinWidth, downHillMaxWidth);
                break;
            }
        }
        GameObject nextObject = GameObject.Instantiate(StandardLevelPart);
        Transform nextObjectGround = nextObject.transform.Find("Ground");
        nextObjectGround.localScale = new Vector3(nextXScale, nextObjectGround.localScale.y, nextObjectGround.localScale.z);
        nextObjectGround.localPosition = new Vector3(nextXScale / 2, nextObjectGround.localPosition.y, 0);
        lastObject = nextObject;
        lastObject.transform.position = nextSpawnPosition;
        previousObjects.Enqueue(lastObject);
        if(previousObjects.Count > maxObjects){
            GameObject.Destroy(previousObjects.Dequeue());
        }
        StartCoroutine(GenerateTimer());
    }
    
}
