using UnityEngine;


public class Player_FallState : PlayerBaseState
{
    public Player_FallState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Update()
    {
        base.Update();

        if (player.isGrounded)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    
}
