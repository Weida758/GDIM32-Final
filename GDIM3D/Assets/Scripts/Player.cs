using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public StateMachine stateMachine { get; private set; }

    private PlayerInputs inputs;
    // -------- States ---------
    [DisplayOnly] public string currentState;
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }

    // --------- Components -------------
    public Animator animator { get; private set; }
    public Rigidbody rb { get; private set; }
    public HealthComponent healthComponent { get; private set; }
    
    [field: SerializeField] public float speed { get; private set; }
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform hitboxOrigin;
    public Vector3 moveDirection { get; private set; }
    private float horizontal, vertical;
    public Vector3 moveInput { get; private set; }
    
    public Transform cam;


    private void Awake()
    {
        stateMachine = new StateMachine();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        inputs = GetComponent<PlayerInputs>();

        idleState = new Player_IdleState(stateMachine, "idle", this);
        moveState = new Player_MoveState(stateMachine, "move", this);
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    
    private void Update()
    {
        currentState = stateMachine.currentState.ToString();
        HandleInput();
        GetCameraDirection();
        stateMachine.UpdateActiveState();
        

    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    private void GetCameraDirection()
    {

        if (moveInput.magnitude >= 0.1f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0f; // strips out up and down from forward
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camForward * vertical + camRight * horizontal;
            moveDirection.Normalize();

            // Smoothly rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * 100f * Time.deltaTime);
        }
    }

    private void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(horizontal, 0f, vertical).normalized;
        
    }
    
    
}