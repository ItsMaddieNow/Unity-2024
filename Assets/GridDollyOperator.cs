using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GridDollyOperator : MonoBehaviour, IGridTurns
{
    public AnimationCurve cameraTransition = new AnimationCurve(
        new Keyframe(0, 0, 1, 1), 
        new Keyframe(1, 1, 1, 1)
    );
    CinemachineSplineDolly cameraDolly;
    public float transitionDuration = 5f;

    void Start(){
        cameraDolly = GetComponent<CinemachineSplineDolly>();
    }
    
    public void PlayerTransition(GamePlayer player)
    {
        switch (player){
            case GamePlayer.one:
                StartCoroutine(SwingToOne());
                break;
            case GamePlayer.two:
                StartCoroutine(SwingToTwo());
                break;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player Token", (int)player));
        }
    }
    IEnumerator SwingToOne(){
        for(float i = 0f; i < 1f; i+=Time.deltaTime/transitionDuration){
            cameraDolly.CameraPosition = cameraTransition.Evaluate(1f-i);
            yield return null;
        }
        cameraDolly.CameraPosition = 0f;
    }

    IEnumerator SwingToTwo(){
        for(float i = 0f; i < 1f; i+=Time.deltaTime/transitionDuration){
            cameraDolly.CameraPosition = cameraTransition.Evaluate(i);
            yield return null;
        }
        cameraDolly.CameraPosition = 1f;
    }

    
}
