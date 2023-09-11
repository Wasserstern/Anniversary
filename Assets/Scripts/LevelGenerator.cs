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
    public GameObject WaterLevelPart;
    public GameObject AnimatedWaterTile;
    public GameObject Crawler;

    // Difficulty and level modifiers
    public float difficulty; // Adjusts spawn chances, mode interval, platform sizes, gap sizes
    public enum LevelMode { flat, cascade, downhill, uphill, crazy}
    [Range (0, 1)]
    public float crawlerChance;
    public float levelModeIntervalInSeconds;
    public float minHeight;
    public float maxHeight;
    public float maxDistance;
    public float minDistance;
    public float flatMaxWidth;
    public float flatMinWidth;
    public float cascadeMaxWidth;
    public float cascadeMinWidth;
    public float downHillMinHeight;
    public float downHillMaxHeight;
    public float downHillMinWidth;
    public float downHillMaxWidth;
    public float upHillMaxHeight;
    public float upHillMinHeight;

    public int levelObjectHeight;
    public int maxObjects;
    public List<LevelMode> enforcedModes;
    bool justSpawnedEnemy;
    public float generationIntervalInSeconds;
    public float difficultyChangeIntervalInSeconds;
    public Queue<GameObject> currentLevelObjects;
    public Queue<GameObject> currentEnemies;
    public LevelMode currentMode;

    public GameObject lastObject;
  
    void Start()
    {
        currentLevelObjects = new Queue<GameObject>();
        currentEnemies = new Queue<GameObject>();
        StartCoroutine(GenerateTimer());
        StartCoroutine(LevelModeTimer());
        // StartCoroutine(DifficultyTimer());
    }

    void Update()
    {
        
    }

    IEnumerator GenerateTimer(){
        yield return new WaitForSeconds(generationIntervalInSeconds);
        Generate();
    }

    IEnumerator LevelModeTimer(){
        if(enforcedModes.Count == 0){
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
            currentMode = (LevelMode)nextLevelModeChoices[Random.Range(0, nextLevelModeChoices.Length)];
            StartCoroutine(LevelModeTimer());
        }
        else{
            yield return new WaitForSeconds(levelModeIntervalInSeconds);
            currentMode = enforcedModes[Random.Range(0, enforcedModes.Count)];
            StartCoroutine(LevelModeTimer());
        }
    }

    public void ChangeDifficulty(int newDifficultyLevel){
        StopCoroutine(LevelModeTimer());
        // Adjust each difficulty level here.
        switch(newDifficultyLevel){
            case 0:{
                break;
            }
            case 1:{
                break;
            }
            case 2:{
                break;
            }
            case 3:{
                break;
            }
            case 4:{
                break;
            }
            case 5:{
                break;
            }
            case 6:{
                break;
            }
            case 7:{
                break;
            }
        }
        StartCoroutine(LevelModeTimer());
    }

    // No longer using this
    [Obsolete("Difficulty no longer changed over time.")]
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
                nextXScale = Random.Range(flatMinWidth, flatMaxWidth);
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
        // Spawn next object, set its sprite size, collider size and position
        GameObject nextObject = GameObject.Instantiate(StandardLevelPart);
        Transform nextObjectGround = nextObject.transform.Find("Ground");
        SpriteRenderer nextObjectRenderer = nextObjectGround.gameObject.GetComponent<SpriteRenderer>();
        nextObjectRenderer.size = new Vector2(nextXScale, levelObjectHeight);
        nextObjectGround.GetComponent<BoxCollider2D>().size = new Vector2(nextXScale, levelObjectHeight);
        nextObjectGround.localPosition = new Vector3(nextXScale / 2, nextObjectGround.localPosition.y - levelObjectHeight / 2, 0);
        // Spawn water object inbetween lastObject and nextObject

        float waterX = lastObject.transform.position.x + lastObjectRenderer.size.x;
        float waterY = lastObject.transform.position.y <= nextSpawnPosition.y ? lastObject.transform.position.y -1 : nextSpawnPosition.y -1;
        Vector2 waterPosition = new Vector2(waterX, waterY);
        GameObject water = GameObject.Instantiate(WaterLevelPart);
        Transform waterGround = water.transform.Find("Ground");
        SpriteRenderer waterRenderer = waterGround.gameObject.GetComponent<SpriteRenderer>();
        float waterXScale = nextSpawnPosition.x - (lastObject.transform.position.x + lastObjectRenderer.size.x);
        waterRenderer.size = new Vector2(waterXScale, levelObjectHeight);
        waterGround.GetComponent<BoxCollider2D>().size = new Vector2(waterXScale, levelObjectHeight);
        waterGround.localPosition = new Vector3(waterXScale / 2, waterGround.localPosition.y - levelObjectHeight / 2, 0);
        water.transform.position = waterPosition;

        // Spawn animated water tiles on top of water surface
        int waterTileCount = Mathf.FloorToInt(waterXScale) +1;
        Vector2 waterTilePosition = new Vector2(waterPosition.x + 0.5f, waterPosition.y + 0.3f);
        for(int i = 0; i < waterTileCount; i++){
            GameObject waterTile = GameObject.Instantiate(AnimatedWaterTile);
            waterTile.transform.position = waterTilePosition;
            waterTilePosition += new Vector2(1, 0);
        }
            
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
