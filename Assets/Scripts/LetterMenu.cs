using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LetterMenu : MonoBehaviour
{
    public Transform gameMessageContainer;
    public AudioSource gameMusicSource;
    public AudioSource letterMenuMusicSource;
    public List<string> letterTexts;
    AllManager allmng;

    VisualElement selectedLetterContainer;
    Button backButton;
    TextField letterTextField;


    String currentText;
    int currentTextIndex;
    public float timePerLetter;
    public float changeVolumeSpeed;
    float currentLetterTime;

    float normalGameVolume;

 
    // Start is called before the first frame update
    void Start()
    {
        allmng = GameObject.Find("AllManager").GetComponent<AllManager>();
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        selectedLetterContainer = root.Q<VisualElement>("SelectedLetterContainer");
        backButton = root.Q<Button>("BackButton");
        letterTextField = root.Q<TextField>("LetterText");
        normalGameVolume = gameMusicSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        if(Input.GetKeyDown(KeyCode.Escape)){
            StopCoroutine(allmng.GameMessageAppear());
            gameMessageContainer.localPosition = new Vector3(-1000, 0, 0);
            if(root.visible){
                root.visible = false;
                Time.timeScale = 1;
            }
            else{
                Time.timeScale = 0;
                List<VisualElement> letters = root.Query<VisualElement>(className: "Letter").ToList();
                for(int i = 0; i < letters.Count; i++){
                    if(allmng.unlockedLetters[i]){
                        letters[i].AddToClassList("LetterUnlocked");
                    }
                }
                root.visible = true;
            }
                CloseLetter();
        }   
        // Letter typewriter effect
        if(selectedLetterContainer.visible && letterTextField.value != currentText){
            if(currentLetterTime  >= timePerLetter){
                letterTextField.value += currentText[currentTextIndex];
                currentTextIndex++;
                currentLetterTime = Mathf.Abs(currentLetterTime - timePerLetter);
            }
            currentLetterTime += Time.unscaledDeltaTime;
            Debug.Log(Time.unscaledDeltaTime);
        }
        else{
            currentTextIndex = 0;
            currentLetterTime = 0;
        }
        // Music damp when paused and other stuff
        if(root.visible){
            if(gameMusicSource.volume > 0f){
                gameMusicSource.volume -= Time.unscaledDeltaTime * changeVolumeSpeed;
            }
            else{
                gameMusicSource.volume = 0f;
                gameMusicSource.Pause();
                if(!letterMenuMusicSource.isPlaying){
                    letterMenuMusicSource.UnPause();
                }
                if(letterMenuMusicSource.volume < 0.75f){
                    letterMenuMusicSource.volume += Time.unscaledDeltaTime * changeVolumeSpeed;
                }
                else{
                    letterMenuMusicSource.volume = 0.75f;
                }
            }
        }
        else{
            letterMenuMusicSource.Pause();
            letterMenuMusicSource.volume = 0f;
            if(!gameMusicSource.isPlaying){
                gameMusicSource.UnPause();
            }
            if(gameMusicSource.volume < normalGameVolume){
                gameMusicSource.volume += Time.unscaledDeltaTime * changeVolumeSpeed;
            }
            else{
                gameMusicSource.volume = normalGameVolume;
            }
        }
    }
    
    private void ClickedLetter(ClickEvent evt, Letter letter){
        if(allmng.unlockedLetters[letter.index]){
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            root.Q<TextField>("LetterText").value = "";
            currentText = letter.text;
            OpenLetter();
            Debug.Log(currentText);
        }
        else{
            // TODO: Add "not unlocked" animation or some sounds
        }
    }

    class Letter{
        public int index;
        public string text;
        public Letter(int index, string text){
            this.index = index;
            this.text = text;
        }
    }

    private void OnEnable(){
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        selectedLetterContainer = root.Q<VisualElement>("SelectedLetterContainer");
        backButton = root.Q<Button>("BackButton");
        CloseLetter();
        root.visible = false;
        List<VisualElement> letters = root.Query(className: "Letter").ToList();
        for(int i = 0; i < letters.Count; i++){
            Letter letter = new Letter(i, letterTexts[i]);
            letters[i].RegisterCallback<ClickEvent, Letter>(ClickedLetter, letter);
        }
        root.Q<Button>("BackButton").clicked += () =>{
            CloseLetter();
        };
        
    }

    void CloseLetter(){
        selectedLetterContainer.visible = false;
        selectedLetterContainer.style.opacity = 0;
        selectedLetterContainer.style.width = Length.Percent(0);
        selectedLetterContainer.style.height = Length.Percent(0);
        backButton.visible = false;
    }
    void OpenLetter(){
        selectedLetterContainer.visible = true;
        selectedLetterContainer.style.opacity = 1;
        selectedLetterContainer.style.width = Length.Percent(40);
        selectedLetterContainer.style.height = Length.Percent(80);
    }

}
