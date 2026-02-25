using UnityEngine;

public class Player : MonoBehaviour
{
    public StateMachine stateMachine { get; private set; }

    public PlayerInputs inputs { get; private set; }

    // -------- States ---------
    [DisplayOnly] public string currentState;
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }

    // --------- Components -------------
    public Animator animator { get; private set; }
    public Rigidbody rb { get; private set; }
    public HealthComponent healthComponent { get; private set; }
    [SerializeField] private GameObject inventoryUI;

    // --------- Data -------------------
    [field: SerializeField] public float speed { get; private set; }
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform hitboxOrigin;
    private bool inventoryOpened = false;
    [DisplayOnly] public bool isGrounded = true;

    // -------- Movement -------------------
    public Vector3 moveDirection { get; private set; }
    private float horizontal, vertical;
    public Vector3 moveInput { get; private set; }
    [field: SerializeField] public float jumpForce { get; private set; }
    
    public Transform cam;
    private OrbitCamera orbitCamera;
    


    private void Awake()
    {
        stateMachine = new StateMachine();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        inputs = GetComponent<PlayerInputs>();

        idleState = new Player_IdleState(stateMachine, "idle", this);
        moveState = new Player_MoveState(stateMachine, "move", this);
        jumpState = new Player_JumpState(stateMachine, "jump", this);
        fallState = new Player_FallState(stateMachine, "fall", this);
        
        healthComponent = GetComponent<HealthComponent>();
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
        Cursor.lockState = CursorLockMode.Locked;
        orbitCamera = FindFirstObjectByType<OrbitCamera>();
    }
    
    
    private void Update()
    {
        currentState = stateMachine.currentState.ToString();
        HandleInput();
        GetCameraDirection();
        stateMachine.UpdateActiveState();

        if (inputs.inventoryPressed)
        {
            inventoryOpened = !inventoryOpened;
            inventoryUI.SetActive(inventoryOpened);
            Cursor.visible =  inventoryOpened;
            Cursor.lockState = inventoryOpened ? CursorLockMode.None : CursorLockMode.Locked;
            
            orbitCamera.enabled = !inventoryOpened;
        }
        

    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    // Return the forward direction that the camera is facing, and rotate the player accordingly
    private void GetCameraDirection()
    {
        // Run when there is movement input
        if (moveInput.magnitude >= 0.1f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0f; // strips out up and down from forward to get horizontal only directions
            camRight.y = 0f; // strips out up and down to get horizontal only directions
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    
    public void SetVelocity()
    {
        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }
    
    
    
}