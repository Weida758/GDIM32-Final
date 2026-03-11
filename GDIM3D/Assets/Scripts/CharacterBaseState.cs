using UnityEngine;

public class CharacterBaseState
{
    protected StateMachine stateMachine;
    protected string animBoolName;
    public bool animationTriggerCalled;

    public CharacterBaseState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        animationTriggerCalled = false;
    }

    public virtual void Exit()
    {
        
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void Update()
    {
        
    }
    
    public void CallAnimationTrigger()
    {
        animationTriggerCalled = true;
        Debug.Log("animation trigger called");
    }
    
}