using UnityEngine;


public class Player_AirState : PlayerBaseState
{
    public Player_AirState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.SetVelocity();
        Debug.Log("Setting velocity");
    }
}
