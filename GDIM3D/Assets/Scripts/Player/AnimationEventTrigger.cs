using System;
using UnityEngine;


public class AnimationEventTrigger : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void CallAnimationTrigger()
    {
        player.stateMachine.currentState.CallAnimationTrigger();
    }
}
