using UnityEngine;


public class Player_JumpState : PlayerBaseState
{
    public Player_JumpState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.rb.AddForce(new Vector3(0f, player.jumpForce, 0f));
        player.isGrounded = false;

    }

    public override void Update()
    {
        base.Update();
        if (player.rb.linearVelocity.y < 0)
        {
            player.stateMachine.ChangeState(player.fallState);
        } 
    }
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
