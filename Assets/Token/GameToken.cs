using System;
using System.Collections;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(Renderer))]
public class GameToken : MonoBehaviour
{
    ITokenState state = new RestingState();
    public GamePlayer colour;
    [SerializeField]
    private Material blinkingMaterial;
    
    [Header("Player 1")]
    public Color Player1BaseColor = new Color(1, 0, 0, 1);
    public Color Player1DropColor = new Color(1, 0, 0, 99f/255f);
    public Color Player1HintColor = new Color(1, 1, 1, 1);
    [SerializeField]
    private Material Player1BaseMaterial;

    [Header("Player 2")]
    public Color Player2BaseColor = new Color(1, 1, 0, 1);
    public Color Player2DropColor = new Color(1, 1, 0, 99f/255f);
    public Color Player2HintColor = new Color(1, 1, 1, 1);

    [SerializeField]
    private Material Player2BaseMaterial;
    private new Renderer renderer;
    
    public void Drop(float targetHeight, BaseGrid.StateCompletion dropCompletion)
    {
        renderer.material = blinkingMaterial;
        renderer.material.SetColor("_Primary_Color", GetBaseColor(colour));
        renderer.material.SetColor("_Secondary_Color", GetDropColor(colour));
        ChangeState(new DroppingState(this, targetHeight, dropCompletion));
        
    }
    // Update is called once per frame
    void Update()
    {
        state.Update(this);
    }

    
    public GamePlayer GetColour()
    {
        return colour;
    }

    public Color GetBaseColor(GamePlayer player){
        switch(player){
            case GamePlayer.one:
                return Player1BaseColor;
            case GamePlayer.two:
                return Player2BaseColor;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player Token", (int)player));
        }
    }
    public Color GetDropColor(GamePlayer player){
        switch(player){
            case GamePlayer.one:
                return Player1DropColor;
            case GamePlayer.two:
                return Player2DropColor;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player Token", (int)player));
        }
    }
    public Color GetHintColor(GamePlayer player){
        switch(player){
            case GamePlayer.one:
                return Player1HintColor;
            case GamePlayer.two:
                return Player2HintColor;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player Token", (int)player));
        }
    }
    public Material GetBaseMaterial(GamePlayer player){
        switch(player){
            case GamePlayer.one:
                return Player1BaseMaterial;
            case GamePlayer.two:
                return Player2BaseMaterial;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player Token", (int)player));
        }
    }

    // Called when object is instantiated unlike 'Start', this is important for the drop coroutine
    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    void ChangeState(ITokenState newState){
        state.Exit(this);
        state = newState;
    }

    interface ITokenState{
        public void Update(GameToken token);
        public void Exit(GameToken token);
    }

    class DroppingState: ITokenState{
        float targetHeight;
        float startHeight;
        float velocity = 0;
        BaseGrid.StateCompletion dropCompletion;
        public DroppingState(GameToken token, float targetHeight, BaseGrid.StateCompletion dropCompletion){
            startHeight = token.transform.localPosition.y;
            this.targetHeight = targetHeight;
            this.dropCompletion = dropCompletion;
        }

        public void Exit(GameToken token)
        {
            token.transform.localPosition = new Vector3(token.transform.localPosition.x, targetHeight, token.transform.localPosition.z);
            token.renderer.material = token.GetBaseMaterial(token.colour);
            dropCompletion.markCompleted();
        }

        public void Update(GameToken token) {
            velocity += Time.deltaTime * -9.81f;
            float nextPos = Time.deltaTime * velocity + token.transform.localPosition.y;
            if (nextPos > targetHeight){
                token.renderer.material.SetFloat("_Opacity", math.unlerp(startHeight, targetHeight, nextPos));
                token.transform.localPosition = new Vector3(token.transform.localPosition.x, nextPos, token.transform.localPosition.z);
            }else {
                token.ChangeState(new RestingState());
            }
            
        }
    }
    class RestingState: ITokenState{
        
        public void Update(GameToken token) {
            
        }
        public void Exit(GameToken token)
        {
            
        }
    }
}

