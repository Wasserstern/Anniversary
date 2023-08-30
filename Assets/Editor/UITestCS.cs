using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UITestCS : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTree;
 

    private void CreateGUI(){
        VisualElement root = rootVisualElement;
        root.Add(visualTree.Instantiate());
    }
}
