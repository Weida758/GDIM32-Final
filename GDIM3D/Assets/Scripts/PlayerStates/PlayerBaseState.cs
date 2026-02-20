using UnityEngine;


public class PlayerBaseState : CharacterBaseState
{
    protected Player player;
    public PlayerBaseState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName)
    {
        this.player = player;
    }


    public override void Enter()
    {
        base.Enter();
        
        player.animator.SetBool(animBoolName, true);
    }

    public override void Update()
    {
        base.Update();
        
        player.animator.SetFloat("magnitude", player.moveInput.magnitude, 0.1f, Time.deltaTime);
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool(animBoolName, false);
    }
}
