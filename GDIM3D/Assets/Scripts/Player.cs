using UnityEngine;

public class Player : MonoBehaviour
{
    public StateMachine stateMachine;
    [SerializeField] private float speed;
    [SerializeField] private Transform hitboxOrigin;
    
}
