using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
public class LevelGenerator : MonoBehaviour
{
    Lara lara;
    public GameObject StandardLevelPart;
    public GameObject WaterLevelPart;
    public GameObject AnimatedWaterTile;
    public GameObject Crawler;
    public GameObject PaperScrapGroup;
    public GameObject Foliage;

    public List<Sprite> grassSprites;
    public List<Sprite> smallFoliageSprites;
    public List<Sprite> bigSunflowerSprites;

    // Difficulty and level modifiers
    public float difficulty; // Adjusts spawn chances, mode interval, platform sizes, gap sizes
    public enum LevelMode { flat, cascade, downhill, uphill, crazy}
    [Range (0, 1)]
    public float paperScrapChance;
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

    public float uphillMinWidth;
    public float upHillMaxWidth;

    public int levelObjectHeight;
    public int maxObjects;
    public List<LevelMode> enforcedModes;
    bool justSpawnedEnemy;
    public float generationIntervalInSeconds;
    public float difficultyChangeIntervalInSeconds;
    public Queue<GameObject> currentLevelObjects;
    public Queue<GameObject> currentEnemies;
    public Queue<GameObject> currentPaperScrapGroups;
    public Queue<GameObject> currentFoliageGroups;
    public LevelMode currentMode;
    public GameObject lastObject;
    [Range(0, 1)]
    public float smallFoliageChance;
    [Range(0, 1)]
    public float bigSunflowerChance;

    int currentDifficulty;
  
    void Start()
    {
        lara = GameObject.Find("Lara").GetComponent<Lara>();
        currentLevelObjects = new Queue<GameObject>();
        currentEnemies = new Queue<GameObject>();
        currentPaperScrapGroups = new Queue<GameObject>();
        currentFoliageGroups = new Queue<GameObject>();
        //StartCoroutine(GenerateTimer());
        StartCoroutine(LevelModeTimer());
        // StartCoroutine(DifficultyTimer());
        Generate(false);
        Generate(true);
        Generate(false);
    }

    void Update()
    {
        
    }

    [Obsolete("Not using a timer for level generation anymore.")]
    IEnumerator GenerateTimer(){
        yield return new WaitForSeconds(generationIntervalInSeconds);
        Generate(false);
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
        currentDifficulty = newDifficultyLevel;
        switch(newDifficultyLevel){
            case 0:{
                // start difficulty set via editor
                break;
            }
            case 1:{
                crawlerChance = 0.8f;
                currentMode = LevelMode.uphill;
                enforcedModes = new List<LevelMode> {LevelMode.uphill};
                upHillMaxWidth = 20;
                uphillMinWidth = 10;
                break;
            }
            case 2:{
                crawlerChance = 1f;
                currentMode = LevelMode.cascade;
                enforcedModes = new List<LevelMode> {LevelMode.cascade};
                cascadeMaxWidth = 20;
                cascadeMinWidth = 10;
                break;
            }
            case 3:{
                currentMode = LevelMode.downhill;
                enforcedModes = new List<LevelMode> {LevelMode.cascade, LevelMode.uphill, LevelMode.downhill};
                cascadeMaxWidth = 15;
                cascadeMinWidth = 7;
                uphillMinWidth = 7;
                upHillMaxWidth = 15;
                downHillMinWidth = 7;
                downHillMaxWidth = 15;
                break;
            }
            case 4:{
                currentMode = LevelMode.flat;
                enforcedModes = new List<LevelMode> {LevelMode.cascade, LevelMode.uphill, LevelMode.downhill, LevelMode.flat};
                cascadeMinWidth = 10;
                uphillMinWidth = 10;
                downHillMinWidth = 10;
                flatMaxWidth = 18;
                flatMinWidth = 10;
                lara.normalSpeed = lara.normalSpeed + 3.5f;
                minDistance = 4.5f;
                maxDistance = 8f;
                break;
            }
            case 5:{
                lara.normalSpeed = lara.normalSpeed + 3f;
                minDistance = 5f;
                maxDistance = 9.5f;
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

    void Generate(bool setGenerator){
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
                nextXScale = Random.Range(uphillMinWidth, upHillMaxWidth);
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
        
        // Spawn paper scrap above ground
        if(Random.Range(0, 1f) <= paperScrapChance){
            // Spawn paper scap pattern
            Vector2 nextPaperScrapPosition = new Vector2(nextSpawnPosition.x + 0.5f, nextSpawnPosition.y + 1.5f);
            GameObject paperScrapGroup = GameObject.Instantiate(PaperScrapGroup, nextPaperScrapPosition, new UnityEngine.Quaternion(0, 0, 0 ,0));
            paperScrapGroup.GetComponent<PaperScrapGroup>().ActivateWithPattern(0, nextXScale);
            currentPaperScrapGroups.Enqueue(paperScrapGroup);
        }

        // Spawn paper scrap above water

        if(Random.Range(0, 1f) <= paperScrapChance){
            Vector2 nextPaperScrapPosition = new Vector2(nextSpawnPosition.x + nextXScale, nextSpawnPosition.y + 1.5f);
            GameObject paperScrapGroup = GameObject.Instantiate(PaperScrapGroup, nextPaperScrapPosition, new UnityEngine.Quaternion(0, 0, 0 ,0));
            paperScrapGroup.GetComponent<PaperScrapGroup>().ActivateWithPattern(3, nextXScale);
            currentPaperScrapGroups.Enqueue(paperScrapGroup);
        }

        SpawnFoliage(nextSpawnPosition, Mathf.FloorToInt(nextXScale));
        
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
        float randomWaterOffset = Random.Range(0.25f, 1f);
        waterGround.localPosition = new Vector3(waterXScale / 2, waterGround.localPosition.y - levelObjectHeight / 2 -randomWaterOffset, 0);
        water.transform.position = waterPosition;

        // Spawn animated water tiles on top of water surface
        int waterTileCount = Mathf.FloorToInt(waterXScale) +1;
        Vector2 waterTilePosition = new Vector2(waterPosition.x + 0.5f, waterPosition.y + 0.3f -randomWaterOffset);
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
        if(currentPaperScrapGroups.Count > maxObjects){
            GameObject.Destroy(currentPaperScrapGroups.Dequeue());
        }
        if(currentFoliageGroups.Count > maxObjects){
            GameObject.Destroy(currentFoliageGroups.Dequeue());
        }
        TryEnemySpawn(nextSpawnPosition, nextXScale);
        if(setGenerator){
            transform.position = nextSpawnPosition;
        }
    }

    void SpawnFoliage(Vector2 groundStartPosition, int groundXScale){
        GameObject foliageGroup = new GameObject("FoliageGroup");
        for(int i = 0; i < groundXScale; i++){
            // Spawn grass
            GameObject grassFoliage = GameObject.Instantiate(Foliage);
            SpriteRenderer grassRenderer = grassFoliage.GetComponent<SpriteRenderer>();
            grassRenderer.sprite = grassSprites[Random.Range(0, grassSprites.Count)];
            grassRenderer.sortingOrder = 1;
            grassFoliage.transform.position = new Vector2(groundStartPosition.x + 0.5f + i, groundStartPosition.y + grassRenderer.bounds.size.y / 2 -0.25f);
            grassFoliage.transform.SetParent(foliageGroup.transform);

            // Spawn small background foliage
            if(Random.RandomRange(0f, 1f) <= smallFoliageChance){
                GameObject backgroundFoliage = GameObject.Instantiate(Foliage);
                SpriteRenderer backgroundFoliageRenderer = backgroundFoliage.GetComponent<SpriteRenderer>();
                backgroundFoliageRenderer.sprite = smallFoliageSprites[Random.Range(0, smallFoliageSprites.Count)];
                backgroundFoliageRenderer.sortingOrder = -1;
                backgroundFoliage.transform.position = new Vector2(groundStartPosition.x + 0.5f + i, groundStartPosition.y + backgroundFoliageRenderer.bounds.size.y / 2 -0.25f);
                backgroundFoliage.transform.SetParent(foliageGroup.transform);
            }
        }
        // Spawn big background sunflowers
            int maxSunflowers = Mathf.FloorToInt(groundXScale / Foliage.GetComponent<SpriteRenderer>().bounds.size.x);
            Vector2 currentSunflowerPosition = groundStartPosition;
            for(int i = 0; i < maxSunflowers; i++){
                if(Random.Range(0f, 1f) <= bigSunflowerChance){
                GameObject bigSunflower = GameObject.Instantiate(Foliage);
                SpriteRenderer bigSunflowerRenderer = bigSunflower.GetComponent<SpriteRenderer>();
                bigSunflowerRenderer.sprite = bigSunflowerSprites[Random.Range(0, bigSunflowerSprites.Count)];
                bigSunflowerRenderer.sortingOrder = -2;
                float xOffset = Random.Range(0.25f, bigSunflowerRenderer.bounds.size.x);
                bigSunflower.transform.position = new Vector2(currentSunflowerPosition.x + xOffset, currentSunflowerPosition.y + bigSunflowerRenderer.bounds.size.y / 2 -0.25f);
                if(xOffset >= bigSunflowerRenderer.bounds.size.x / 2){
                    i++;
                    currentSunflowerPosition = new Vector2(currentSunflowerPosition.x + bigSunflowerRenderer.bounds.size.x, currentSunflowerPosition.y);
                }
                currentSunflowerPosition = new Vector2(currentSunflowerPosition.x + bigSunflowerRenderer.bounds.size.x, currentSunflowerPosition.y);
                bigSunflower.transform.SetParent(foliageGroup.transform);
                }

            }
            
    }

    void TryEnemySpawn(Vector2 groundPosition, float groundWidth){
        bool spawningEnemy = false;
        GameObject nextEnemy = null;
        Vector2 enemyPosition = new Vector2();
        if(!justSpawnedEnemy){
            if(Random.Range(0f, 1f) <= crawlerChance){
                enemyPosition = new Vector2(groundPosition.x + 0.5f + Random.Range(0f, groundWidth -1f), groundPosition.y -0.25f);
                nextEnemy = GameObject.Instantiate(Crawler, enemyPosition, new UnityEngine.Quaternion(0, 0, 0, 0));
                nextEnemy.SendMessage("SetCrawlerLeft", Random.Range(difficulty / 3, difficulty));
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
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Lara")){
            Generate(false);
            Generate(true);
            Generate(false);
        }
    }
   
    
}
