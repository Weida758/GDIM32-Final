using UnityEngine;

public class Player_AttackState : PlayerBaseState
{
    public Player_AttackState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    { }


    public override void Enter()
    {
        base.Enter();
        
        Debug.Log("Attack State entered");
    }

    public override void Update()
    {
        base.Update();

        if (animationTriggerCalled)
        {
            player.stateMachine.ChangeState(player.idleState);
            Debug.Log("Changing from attack to idle state");
            return;
        }
    }
    
    
    
    
    
    
}
