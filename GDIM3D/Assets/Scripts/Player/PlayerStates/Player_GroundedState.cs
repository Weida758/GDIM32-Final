using UnityEngine;


public class Player_GroundedState : PlayerBaseState
{
    public Player_GroundedState(StateMachine stateMachine, string animBoolName, Player player) : base(stateMachine, animBoolName, player)
    {
        this.player = player;
    }

    public override void Update()
    {
        base.Update();

        if (player.inputs.jumpButtonPressed && player.isGrounded)
        {
            player.stateMachine.ChangeState(player.jumpState);
        }
    }
}
