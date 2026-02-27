using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();

        player.rb.linearVelocity = new Vector3(0f, player.rb.linearVelocity.y, 0f);
    }



    public override void Update()
    {
        base.Update();

        if (player.moveInput != Vector3.zero)
        {
            stateMachine.ChangeState(player.moveState);
            Debug.Log("changing to move state");
        }
    }



    public override void Exit()
    {
        base.Exit();
    }
}