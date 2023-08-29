using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject StandardLevelPart;

    public enum LevelMode { flat, cascade, downhill, uphill, crazy}
    public float difficulty;
    public float generationIntervalInSeconds;
    public Stack<GameObject> previousObjects;
    public LevelMode currentMode;

    public float minHeight;
    public float maxHeight;
    public float maxDistance;
    
    public float minDistance;

    
    public GameObject lastObject;
    [SerializeField]


    public float cascadeMaxWidth;
    public float cascadeMinWidth;
    void Start()
    {
        previousObjects = new Stack<GameObject>();
        StartCoroutine(GenerateTimer());
    }

    void Update()
    {
        
    }

    IEnumerator GenerateTimer(){
        yield return new WaitForSeconds(generationIntervalInSeconds);
        Generate();
    }

    void Generate(){
        switch(currentMode){
            case LevelMode.flat:{
                break;
            }
            case LevelMode.cascade:{

                Vector2 nextSpawnPosition = new Vector2();
                Transform lastObjectGround = lastObject.transform.Find("Ground");
                Debug.Log(lastObjectGround.localScale.x / 2);
                if(Random.Range(0, 2) == 0){
                    // Spawn next object above current height
                    nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectGround.localScale.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y + Random.Range(0, maxHeight));
                }
                else{
                    // Spawn next object below current height
                    nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectGround.localScale.x  + Random.Range(minDistance, maxDistance), lastObject.transform.position.y - Random.Range(0, minHeight));
                }
                GameObject nextObject = GameObject.Instantiate(StandardLevelPart);
                float nextXScale = Random.Range(cascadeMinWidth, cascadeMaxWidth);
                Transform nextObjectGround = nextObject.transform.Find("Ground");
                nextObjectGround.localScale = new Vector3(nextXScale, nextObjectGround.localScale.y, nextObjectGround.localScale.z);
                nextObjectGround.localPosition = new Vector3(nextXScale / 2, nextObjectGround.localPosition.y, 0);
                lastObject = nextObject;
                lastObject.transform.position = nextSpawnPosition;
                previousObjects.Push(lastObject);

                break;
            }
            case LevelMode.downhill:{
                break;
            }
            case LevelMode.uphill:{
                break;
            }
            case LevelMode.crazy:{
                break;
            }
        }
        StartCoroutine(GenerateTimer());
    }
    
}
