using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GridDollyOperator : MonoBehaviour, IGridTurns
{
    IOperatorState state = new RestingState(); 
    public AnimationCurve cameraTransition = new AnimationCurve(
        new Keyframe(0, 0, 1, 1), 
        new Keyframe(1, 1, 1, 1)
    );
    CinemachineSplineDolly cameraDolly;
    public float transitionDuration = 5f;

    void Start(){
        cameraDolly = GetComponent<CinemachineSplineDolly>();
    }

    void Update() {
        state.Update(this);
    }
    
    public void PlayerTransition(GamePlayer player, BaseGrid.StateCompletion completion)
    {
        switch (player){
            case GamePlayer.one:
                this.ChangeState(new SwingToOneState(completion));
                break;
            case GamePlayer.two:
                this.ChangeState(new SwingToTwoState(completion));
                break;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player Token", (int)player));
        }
    }
    
    /*IEnumerator SwingToOne(){
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
    }*/
    
    void ChangeState(IOperatorState newState){
        state.Exit(this);
        state = newState;
    }

    public interface IOperatorState{
        public void Update(GridDollyOperator dollyOperator);
        public void Exit(GridDollyOperator dollyOperator);
    }

    public class SwingToOneState: IOperatorState{
        float i = 0f;
        BaseGrid.StateCompletion completionMonitor;
        public SwingToOneState(BaseGrid.StateCompletion completion){
            this.completionMonitor = completion;
        }

        public void Update(GridDollyOperator dollyOperator){
            i+=Time.deltaTime/dollyOperator.transitionDuration;
            if (i<1f){
                dollyOperator.cameraDolly.CameraPosition = dollyOperator.cameraTransition.Evaluate(1f-i);
            }
            else{
                dollyOperator.ChangeState(new RestingState());
            }
            
        }

        public void Exit(GridDollyOperator dollyOperator)
        {
            completionMonitor.markCompleted();
            dollyOperator.cameraDolly.CameraPosition = 0f;
        }
    }
    public class SwingToTwoState: IOperatorState{
        float i = 0f;
        BaseGrid.StateCompletion completionMonitor;
        public SwingToTwoState(BaseGrid.StateCompletion completion){
            this.completionMonitor = completion;
        }
        public void Update(GridDollyOperator dollyOperator){
            i+=Time.deltaTime/dollyOperator.transitionDuration;
            if (i<1f){
                dollyOperator.cameraDolly.CameraPosition = dollyOperator.cameraTransition.Evaluate(i);;
            }
            else{
                dollyOperator.ChangeState(new RestingState());
            }
            
        }

        public void Exit(GridDollyOperator dollyOperator)
        {
            completionMonitor.markCompleted();
            dollyOperator.cameraDolly.CameraPosition = 1f;
        }
    }
    public class RestingState: IOperatorState{

        public void Update(GridDollyOperator dollyOperator){}

        public void Exit(GridDollyOperator dollyOperator){}
    }
}
