using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllManager : MonoBehaviour
{
    LevelGenerator levelGenerator;
    public float yCameraOffset;
    int currentPaperScraps;
    public int goalPaperScrapCount;
    public List<int> paperScrapGoals;
    public int currentDifficulty;
    int goalIndex;

    public void CollectPaperScrap(int count){
        currentPaperScraps += count;
        if(currentPaperScraps >= goalPaperScrapCount){
            int overflowScraps = currentPaperScraps - goalPaperScrapCount;
            currentPaperScraps = overflowScraps;
            goalIndex += 1;
            goalPaperScrapCount = paperScrapGoals[goalIndex];
            currentDifficulty++;
            levelGenerator.ChangeDifficulty(currentDifficulty);
        }
    }
    void Start()
    {
        levelGenerator = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();

    }

    void Update()
    {
        
    }
}
