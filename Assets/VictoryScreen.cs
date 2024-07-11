using System;
using UnityEngine;
using UnityEngine.UIElements;

public class VictoryScreen : MonoBehaviour, IGridEnd
{
    public BaseGrid grid;
    private UIDocument _doc;
    private Label _playerVictoryText;
    private Button _quit;
    private Button _retry;
    private VisualElement _base;
    private VisualElement _content;
    
    void OnEnable(){
        _doc = GetComponent<UIDocument>();

        GetUiElements();

        _quit.RegisterCallback<ClickEvent>(Quit);
        _retry.RegisterCallback<ClickEvent>(Retry);
        _content.RegisterCallback<TransitionEndEvent>(OnAnimationFinish);
    }
    void GetUiElements(){
        _base = _doc.rootVisualElement.Q<VisualElement>("Base");
        _content = _base.Q<VisualElement>("Content");
        _playerVictoryText = _content.Q<Label>("EndText");
        _quit = _content.Q<Button>("Quit");
        _retry = _content.Q<Button>("Retry");
        _playerVictoryText = _content.Q<Label>("EndText");
    }
    void OnDisable(){
        _quit.UnregisterCallback<ClickEvent>(Quit);
        _retry.UnregisterCallback<ClickEvent>(Retry);
        _content.UnregisterCallback<TransitionEndEvent>(OnAnimationFinish);
    }

    void OnAnimationFinish(TransitionEndEvent evt){
        if(evt.stylePropertyNames.Contains(new StylePropertyName("scale"))){
            print("Animation Identified");
            CloseUi();
            grid.Reset();
        }
    }
    void Quit(ClickEvent evt){
        // Application.Quit Doesn't work in the Editor So we Print this 
        print("Quit Game");
        Application.Quit();
    }
    void CloseUi(){
        gameObject.SetActive(false);
    }
    void Retry(ClickEvent evt){
        DisableAnimation();
    }
    void EnableAnimation(){
        _base.AddToClassList("background-active");
        _base.RemoveFromClassList("background-inactive");
    }
    void DisableAnimation(){
        _base.RemoveFromClassList("background-active");
        _base.AddToClassList("background-inactive");
    }
    void EnableUI(){
        gameObject.SetActive(true);
        //GetUiElements();

        EnableAnimation();
    }
    public void Victory(GamePlayer player){
        EnableUI();
        _playerVictoryText.text = string.Format("Player {0} Wins", player==GamePlayer.one? "One": "Two");
    }
    public void End(){
        EnableUI();
        _playerVictoryText.text = "Draw";
    }
    public void Reset(){
        DisableAnimation();
        gameObject.SetActive(false);

    }

}
