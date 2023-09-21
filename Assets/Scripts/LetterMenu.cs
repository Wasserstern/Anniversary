using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LetterMenu : MonoBehaviour
{
    public List<string> letterTexts;
    AllManager allmng;

    VisualElement selectedLetterContainer;
    Button backButton;
 
    // Start is called before the first frame update
    void Start()
    {
        allmng = GameObject.Find("AllManager").GetComponent<AllManager>();
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        selectedLetterContainer = root.Q<VisualElement>("SelectedLetterContainer");
        backButton = root.Q<Button>("BackButton");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            if(root.visible){
                root.visible = false;
                Time.timeScale = 1;
            }
            else{
                Time.timeScale = 0;
                root.visible = true;
            }
                CloseLetter();
        }   
    }

    
    private void ClickedLetter(ClickEvent evt, Letter letter){
        if(allmng.unlockedLetters[letter.index]){
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            root.Q<TextField>("LetterText").value = letter.text;
            OpenLetter();
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
