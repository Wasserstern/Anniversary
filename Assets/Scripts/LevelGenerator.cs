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
    public GameObject Crawler;

    public enum LevelMode { flat, cascade, downhill, uphill, crazy}
    public int maxObjects;
    public float difficulty;
    [Range (0, 1)]
    public float crawlerChance;
    bool justSpawnedEnemy;
    public float generationIntervalInSeconds;
    public float difficultyChangeIntervalInSeconds;
    public float levelModeIntervalInSeconds;
    public Queue<GameObject> currentLevelObjects;
    public Queue<GameObject> currentEnemies;
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
        currentLevelObjects = new Queue<GameObject>();
        currentEnemies = new Queue<GameObject>();
        StartCoroutine(GenerateTimer());
        StartCoroutine(LevelModeTimer());
        StartCoroutine(DifficultyTimer());
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
    IEnumerator DifficultyTimer(){
        yield return new WaitForSeconds(difficultyChangeIntervalInSeconds);
        difficulty +=1;

        if(cascadeMinWidth / difficulty >= 2f){
            cascadeMinWidth = cascadeMinWidth / difficulty;
        }
        if(cascadeMaxWidth / difficulty >= 5f){
            cascadeMaxWidth = cascadeMaxWidth / difficulty;
        }

        if(crawlerChance <= 0.8){
            crawlerChance += 0.1f;
        }


        StartCoroutine(DifficultyTimer());
    }

    void Generate(){
        Vector2 nextSpawnPosition = new Vector2();
        Transform lastObjectGround = lastObject.transform.Find("Ground");
        SpriteRenderer lastObjectRenderer = lastObjectGround.gameObject.GetComponent<SpriteRenderer>();
        float nextXScale = 1f;
        switch(currentMode){
            case LevelMode.flat:{
                nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectRenderer.size.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y);
                nextXScale = Random.Range(cascadeMinWidth, cascadeMaxWidth);
                break;
            }
            case LevelMode.cascade:{


                if(UnityEngine.Random.Range(0, 2) == 0){
                    // Spawn next object above current height
                    nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectRenderer.size.x + UnityEngine.Random.Range(minDistance, maxDistance), lastObject.transform.position.y + Random.Range(0, maxHeight));
                }
                else{
                    // Spawn next object below current height
                    nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectRenderer.size.x  + UnityEngine.Random.Range(minDistance, maxDistance), lastObject.transform.position.y - Random.Range(0, minHeight));
                }
                nextXScale = UnityEngine.Random.Range(cascadeMinWidth, cascadeMaxWidth);
                

                break;
            }
            case LevelMode.downhill:{
                nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectRenderer.size.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y - Random.Range(downHillMinHeight, downHillMaxHeight));
                nextXScale = Random.Range(downHillMinWidth, downHillMaxWidth);
                break;
            }
            case LevelMode.uphill:{
                nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectRenderer.size.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y + Random.Range(upHillMinHeight, upHillMaxHeight));
                nextXScale = Random.Range(downHillMinWidth, downHillMaxWidth);
                break;
            }
            case LevelMode.crazy:{
                 nextSpawnPosition = new Vector2(lastObject.transform.position.x + lastObjectRenderer.size.x + Random.Range(minDistance, maxDistance), lastObject.transform.position.y + Random.Range(upHillMinHeight, upHillMaxHeight));
                nextXScale = Random.Range(downHillMinWidth, downHillMaxWidth);
                break;
            }
        }
        GameObject nextObject = GameObject.Instantiate(StandardLevelPart);
        Transform nextObjectGround = nextObject.transform.Find("Ground");
        SpriteRenderer nextObjectRenderer = nextObjectGround.gameObject.GetComponent<SpriteRenderer>();
        nextObjectRenderer.size = new Vector2(nextXScale, nextObjectRenderer.size.y);
        nextObjectGround.localPosition = new Vector3(nextXScale / 2, nextObjectGround.localPosition.y, 0);
        lastObject = nextObject;
        lastObject.transform.position = nextSpawnPosition;
        currentLevelObjects.Enqueue(lastObject);
        if(currentLevelObjects.Count > maxObjects){
            GameObject.Destroy(currentLevelObjects.Dequeue());
        }
        TryEnemySpawn(nextSpawnPosition, nextXScale);
        StartCoroutine(GenerateTimer());
    }

    void TryEnemySpawn(Vector2 groundPosition, float groundWidth){
        bool spawningEnemy = false;
        GameObject nextEnemy = null;
        Vector2 enemyPosition = new Vector2();
        if(!justSpawnedEnemy){
            if(Random.Range(0f, 1f) <= crawlerChance){
                enemyPosition = new Vector2(groundPosition.x + 0.5f + Random.Range(0f, groundWidth -1f), groundPosition.y + 0.5f);
                nextEnemy = GameObject.Instantiate(Crawler, enemyPosition, new UnityEngine.Quaternion(0, 0, 0, 0));
                nextEnemy.SendMessage("SetCrawler", Random.Range(difficulty / 3, difficulty));
                spawningEnemy = true;
            }
            justSpawnedEnemy = true;
        }
        else{
            justSpawnedEnemy = false;
        }
        

        if(nextEnemy != null){
            currentEnemies.Enqueue(nextEnemy);
            if(currentEnemies.Count > maxObjects){
                GameObject.Destroy(currentEnemies.Dequeue());
            }
        }
    }
    
}
