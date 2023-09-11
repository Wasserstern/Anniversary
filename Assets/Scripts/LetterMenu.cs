using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LetterMenu : MonoBehaviour
{
    public List<string> letterTexts;
    AllManager allmng;
 
    // Start is called before the first frame update
    void Start()
    {
        allmng = GameObject.Find("AllManager").GetComponent<AllManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            if(root.visible){
                root.Q<VisualElement>("SelectedLetterContainer").visible = false;
                root.visible = false;
                Time.timeScale = 1;
            }
            else{
                Time.timeScale = 0;
                root.visible = true;
            }
        }   
    }

    
    private void ClickedLetter(ClickEvent evt, string letterText){
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<TextField>("LetterText").value = letterText;
        root.Q<VisualElement>("SelectedLetterContainer").visible = true;
    }

    private void OnEnable(){
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.visible = false;
        List<VisualElement> letters = root.Query(className: "Letter").ToList();

        for(int i = 0; i < letters.Count; i++){
            letters[i].RegisterCallback<ClickEvent, string>(ClickedLetter, letterTexts[i]);
        }
        root.Q<Button>("BackButton").clicked += () =>{
            root.Q<VisualElement>("SelectedLetterContainer").visible = false;
        };
    }

}
