using UnityEngine;


public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    {
    }

    public override void Update()
    {
        base.Update();
        if (player.moveInput == Vector3.zero)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (player.inventoryOpened) return;
        player.SetVelocity();
    }
}
