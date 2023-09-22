using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;


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

    public bool[] unlockedLetters;
    int unlockIndex;
    
    
    
    
    // PlayerPref values

    [SerializeField]
    int paperHighScore;
    [SerializeField]
    float distanceHighScore;

    // UI stuff

    public TextMeshProUGUI paperScrapsText;
    public TextMeshProUGUI goalPaperScrapsText;
    public RectTransform gameMessageContainer;
    public float gameMessageAppearTime;


    public IEnumerator GameMessageAppear(){
        float startTime = Time.time;
        float elapsedTime = 0f;
        while(Time.time - startTime < gameMessageAppearTime){
            float f = elapsedTime / gameMessageAppearTime;
            f = EaseFunctions.easeInCubic(f);
            gameMessageContainer.localPosition = Vector3.Lerp(new Vector3(-1000, 154.7f, 0), new Vector3(0, 154.7f, 0), f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameMessageContainer.localPosition = new Vector3(0, 154.7f, 0);
        yield return new WaitForSeconds(4f);
        elapsedTime = 0f;
        startTime = Time.time;
        while(Time.time - startTime < gameMessageAppearTime){
            float f = elapsedTime / gameMessageAppearTime;
            f = EaseFunctions.easeInCubic(f);
            gameMessageContainer.localPosition = Vector3.Lerp(new Vector3(0,154.7f, 0), new Vector3(-1000, 154.7f, 0), f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameMessageContainer.localPosition = new Vector3(-1000, 154.7f ,0);
    }
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

            if(unlockIndex < unlockedLetters.Length){
                if(!unlockedLetters[unlockIndex]){
                    unlockedLetters[unlockIndex] = true;
                    PlayerPrefs.SetInt("unlockedLetters" + unlockIndex.ToString(), 1);
                    unlockIndex++;
                    PlayerPrefs.Save();

                    //TODO: Display: "Unlocked letter part! Press Escape to view unlocked letters."
                    StartCoroutine(GameMessageAppear());
                }
                
            }

        }
        if(currentPaperScraps >= paperHighScore){
            paperHighScore = currentPaperScraps;
            PlayerPrefs.SetInt("paperHightScore", paperHighScore);
        }
        paperScrapsText.text = currentPaperScraps.ToString();
        goalPaperScrapsText.text = paperScrapGoals[goalIndex].ToString();
    }
    void Start()
    {
        // Change array size to change letter count
        unlockedLetters = new bool[8];
        levelGenerator = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
        if(PlayerPrefs.HasKey("paperHighScore")){
            paperHighScore = PlayerPrefs.GetInt("paperHighScore");

        }
        else{
            paperHighScore = 0;
            PlayerPrefs.SetInt("paperHighScore", 0);
            PlayerPrefs.Save();
        }

        if(PlayerPrefs.HasKey("distanceHighScore")){
            distanceHighScore = PlayerPrefs.GetFloat("distanceHighScore");
        }
        else{
            PlayerPrefs.SetFloat("distanceHighScore", 0f);
            PlayerPrefs.Save();
        }

        for(int i = 0; i < unlockedLetters.Length; i++){
            if(PlayerPrefs.HasKey("unlockedLetter" + i.ToString())){
                Debug.Log("Found playerPrefs:" + PlayerPrefs.GetInt("unlockedLetter" + i.ToString()));
                unlockedLetters[i] = PlayerPrefs.GetInt("unlockedLetter" + i.ToString()) == 1;
            }
            else{
                PlayerPrefs.SetInt("unlockedLetters" + i.ToString(), 0);
                PlayerPrefs.Save();
            }
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
            for(int i = 0; i < scoreBreaks ; i++){
                currentPaperScraps = paperHighScore - sum;
                goalIndex = scoreBreaks;
            }
        }
        goalPaperScrapCount = paperScrapGoals[goalIndex];
        goalPaperScrapsText.text = paperScrapGoals[goalIndex].ToString();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.O)){
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs deleted!");
            StartCoroutine(GameObject.Find("Lara").GetComponent<Lara>().Retry());
        }
    }
}
