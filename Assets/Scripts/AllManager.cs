using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AllManager : MonoBehaviour
{
    LevelGenerator levelGenerator;
    public float yCameraOffset;
    int currentPaperScraps;
    public int goalPaperScrapCount;
    public List<int> paperScrapGoals;
    public int peakGoal;
    public int currentDifficulty;
    int goalIndex;
    
    // PlayerPref values

    int paperHighScore;
    float distanceHighScore;

    // UI stuff

    public TextMeshProUGUI paperScrapsText;



    public void CollectPaperScrap(int count){
        currentPaperScraps += count;
        if(currentPaperScraps >= goalPaperScrapCount){
            int overflowScraps = currentPaperScraps - goalPaperScrapCount;
            currentPaperScraps = overflowScraps;
            goalIndex += 1;
            if(goalIndex >= paperScrapGoals.Count){
                goalPaperScrapCount = peakGoal;
            }
            else{
                goalPaperScrapCount = paperScrapGoals[goalIndex];
            }
            currentDifficulty++;
            levelGenerator.ChangeDifficulty(currentDifficulty);
        }
        paperScrapsText.text = currentPaperScraps.ToString();
    }
    void Start()
    {
        levelGenerator = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
        if(PlayerPrefs.HasKey("paperHighScore")){
            paperHighScore = PlayerPrefs.GetInt("paperHighScore");
        }
        else{
            paperHighScore = 0;
            PlayerPrefs.SetInt("paperHighScore", 0);
        }

        if(PlayerPrefs.HasKey("distanceHighScore")){
            distanceHighScore = PlayerPrefs.GetFloat("distanceHighScore");
        }
        else{
            PlayerPrefs.SetFloat("distanceHighScore", 0f);
        }
        
        int scoreBreaks = 0;
        int sum = 0;
        foreach(int goal in paperScrapGoals){
            sum += goal;
            if(sum >= paperHighScore){
                break;
            }
            else{
                scoreBreaks++;
            }
        }

        if(scoreBreaks >= paperScrapGoals.Count){
            // Peak reached. No goals left. Infinite collection of paper scraps now.
            currentPaperScraps = 0;
        }
        else{
            // Still goals to reach. Not all letters unlocked.
            for(int i = 0; i < scoreBreaks; i++){
                currentPaperScraps = paperHighScore - sum;
                goalIndex = scoreBreaks;
            }
        }
        goalPaperScrapCount = paperScrapGoals[goalIndex];
    }

    void Update()
    {
        
    }
}
