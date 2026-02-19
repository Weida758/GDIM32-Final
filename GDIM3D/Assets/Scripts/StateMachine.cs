using UnityEngine;

public class StateMachine
{
    public CharacterBaseState currentState { get; private set; }

    public void Initialize(CharacterBaseState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(CharacterBaseState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }

    public void FixedUpdateActiveState()
    {
        currentState.FixedUpdate();
    }
}