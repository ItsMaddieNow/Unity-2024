using System;
using System.Collections;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(Renderer))]
public class GameToken : MonoBehaviour
{
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
    private Renderer renderer;
    
    public void Drop(float targetHeight, BaseGrid.CompletionCount dropCompletion)
    {
        renderer.material = blinkingMaterial;
        renderer.material.SetColor("_Primary_Color", GetBaseColor(colour));
        renderer.material.SetColor("_Secondary_Color", GetDropColor(colour));
        StartCoroutine(DropCoroutine(targetHeight, dropCompletion));
    }
    IEnumerator DropCoroutine(float targetHeight, BaseGrid.CompletionCount dropCompletion)
    {
        float startHeight = transform.localPosition.y;
        float velocity = 0;
        while (true)
        {
            velocity += Time.deltaTime * -9.81f;
            float nextPos = Time.deltaTime * velocity + transform.localPosition.y;
            if (nextPos <= targetHeight) break;
            renderer.material.SetFloat("_Opacity", math.unlerp(startHeight, targetHeight, nextPos));
            transform.localPosition = new Vector3(transform.localPosition.x, nextPos, transform.localPosition.z);
            yield return null;
        }
        dropCompletion.markCompleted();
        transform.localPosition = new Vector3(transform.localPosition.x, targetHeight, transform.localPosition.z);
        renderer.material = GetBaseMaterial(colour);
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

    // Update is called once per frame
    void Update()
    {
        
    }

}
