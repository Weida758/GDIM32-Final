using System;
using UnityEngine;
using UnityEngine.UI;
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
    private PlayerInventory inventory;

    // --------- Data -------------------
    [field: SerializeField] public float speed { get; private set; }
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform hitboxOrigin;
    /// <summary> Whether the inventory panel is currently open. Used to toggle cursor lock and camera control. </summary>
    public bool inventoryOpened { get; private set; } = false;
    [DisplayOnly] public bool isGrounded = true;

    // -------- Movement -------------------
    /// <summary> The camera-relative direction the player should move toward. Zero when there is no input. </summary>
    public Vector3 moveDirection { get; private set; }
    private float horizontal, vertical;
    /// <summary> Raw movement input as a normalized vector (horizontal, 0, vertical). </summary>
    public Vector3 moveInput { get; private set; }
    [field: SerializeField] public float jumpForce { get; private set; }
    
    public Transform cam;
    private OrbitCamera orbitCamera;
    private Camera mainCam;

    // -------- Interaction -------------------
    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.5f;
    [SerializeField] private float raycastMaxDistance = 50f;
    [SerializeField] private LayerMask interactableMask;
    /// <summary> The item currently under the player's crosshair and within interaction range. Null if nothing is targeted. </summary>
    public Item targetedItem { get; private set; }
    public event Action OnInventoryOpen;
    public event Action OnInventoryClose;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private float groundCheckOffset = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;


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
        inventory = GetComponent<PlayerInventory>();
        
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
        Cursor.lockState = CursorLockMode.Locked;
        orbitCamera = FindFirstObjectByType<OrbitCamera>();
        if (orbitCamera) mainCam = orbitCamera.GetComponent<Camera>();
        else Debug.LogError("No Orbit camera");
    }
    
    
    private void Update()
    {
        currentState = stateMachine.currentState.ToString();
        HandleInput();
        GetCameraDirection();
        CheckGround();
        stateMachine.UpdateActiveState();

        if (inputs.inventoryPressed)
        {
            inventoryOpened = !inventoryOpened;
            inventoryUI.SetActive(inventoryOpened);
            
            if (inventoryOpened) OnInventoryOpen?.Invoke();
            else OnInventoryClose?.Invoke();
            
            Cursor.visible =  inventoryOpened;
            Cursor.lockState = inventoryOpened ? CursorLockMode.None : CursorLockMode.Locked;
            
            orbitCamera.enabled = !inventoryOpened;
        }
        
        HandleInteraction();

    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    // Converts raw input into a camera-relative move direction so the player
    // moves relative to where the camera is facing rather than world axes.
    // Also smoothly rotates the player model to face that direction.
    // When there is no input, moveDirection is zeroed out so no residual
    // movement carries over
    private void GetCameraDirection()
    {
        if (moveInput.magnitude >= 0.1f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camForward * vertical + camRight * horizontal;
            moveDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * 100f * Time.deltaTime);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }

    private void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(horizontal, 0f, vertical).normalized;
        
    }
    
    /// <summary>
    /// Applies horizontal movement based on <see cref="moveDirection"/> and <see cref="speed"/>
    /// while preserving the current vertical velocity (gravity / jump force).
    /// Called from state FixedUpdate methods.
    /// </summary>
    public void SetVelocity()
    {
        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }
    
    // Casts a ray from the screen center to detect interactable items.
    // Only registers a hit if the item is within interactionRange of the player,
    // preventing interaction with distant objects that happen to be under the crosshair.
    private void HandleInteraction()
    {
        targetedItem = null;
        
        if (mainCam == null) return;
        
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, raycastMaxDistance, interactableMask, QueryTriggerInteraction.Collide))
        {
            Item item = hit.collider.GetComponent<Item>();
            
            if (item != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, item.transform.position);
                if (distanceToPlayer <= interactionRange)
                {
                    targetedItem = item;
                }
            }
        }

        if (targetedItem != null && Input.GetMouseButtonDown(0))
        {
            targetedItem.Collect(inventory);
        }
    }

    private void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.up * groundCheckOffset;
        isGrounded = Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out _,
            groundCheckDistance, groundMask);
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * groundCheckOffset;
        Vector3 end = origin + Vector3.down * groundCheckDistance;

        Gizmos.color = isGrounded ? Color.green : Color.red;

        // Sphere at start of cast
        Gizmos.DrawWireSphere(origin, groundCheckRadius);

        // Sphere at end of cast
        Gizmos.DrawWireSphere(end, groundCheckRadius);

        // Line connecting them
        Gizmos.DrawLine(origin, end);
    }
    
    
    
}