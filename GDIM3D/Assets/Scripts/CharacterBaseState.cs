using UnityEngine;

public class CharacterBaseState
{
    protected StateMachine stateMachine;
    protected string animBoolName;

    public CharacterBaseState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        
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
    
}