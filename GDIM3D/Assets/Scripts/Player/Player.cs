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
    public Player_AttackState attackState  { get; private set; }

    // --------- Components -------------
    public Animator animator { get; private set; }
    public Rigidbody rb { get; private set; }
    [field: SerializeField] public HealthComponent healthComponent { get; private set; }
    [field: SerializeField] public WeaponComponent weaponComponent { get; private set; }
    [SerializeField] private GameObject inventoryUI;
    private PlayerInventory inventory;

    // --------- Data -------------------
    [field: SerializeField] public float speed { get; private set; }
    [SerializeField] private float turnSpeed = 7f;
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
    /// <summary> The NPC currently under the player's crosshair and within interaction range. Null if nothing is targeted. </summary>
    public NPCDialogue targetedNPC { get; private set; }
    public event Action OnInventoryOpen;
    public event Action OnInventoryClose;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private float groundCheckOffset = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    // -------- Dialogue -------------------
    private DialogueSystem dialogueSystem;
    
    // ---------- Quest Menu --------------
    [SerializeField] private GameObject questMenuUI;
    private bool questMenuOpened = false;


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
        attackState = new Player_AttackState(stateMachine, "attack", this);
        
        inventory = GetComponent<PlayerInventory>();
        
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
        Cursor.lockState = CursorLockMode.Locked;
        orbitCamera = FindFirstObjectByType<OrbitCamera>();
        if (orbitCamera) mainCam = orbitCamera.GetComponent<Camera>();
        else Debug.LogError("No Orbit camera");

        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        
        if (questMenuUI != null)
        {
            questMenuUI.SetActive(false);
        }
    }
    
    
    private void Update()
    {
        currentState = stateMachine.currentState.ToString();
        
        // Block all gameplay input while dialogue is active
        if (dialogueSystem != null && dialogueSystem.IsDialogueActive)
        {
            moveInput = Vector3.zero;
            moveDirection = Vector3.zero;
            horizontal = 0f;
            vertical = 0f;
            targetedItem = null;
            targetedNPC = null;

            if (stateMachine.currentState != idleState)
            {
                stateMachine.ChangeState(idleState);
            }

            animator.SetFloat("magnitude", 0f);

            return;
        }
        
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
        
        if (Input.GetKeyDown(KeyCode.J) && questMenuUI != null)
        {
            questMenuOpened = !questMenuOpened;
            questMenuUI.SetActive(questMenuOpened);

            Cursor.visible = questMenuOpened;
            Cursor.lockState = questMenuOpened ? CursorLockMode.None : CursorLockMode.Locked;
            orbitCamera.enabled = !questMenuOpened;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            healthComponent.Take20Damage();
        }
        
        HandleInteraction();

    }

    private void FixedUpdate()
    {
        // Don't update state machine physics during dialogue
        if (dialogueSystem != null && dialogueSystem.IsDialogueActive) return;
        
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
    
    // Casts a ray from the screen center to detect interactable items and NPCs.
    // Only registers a hit if the object is within interactionRange of the player,
    // preventing interaction with distant objects that happen to be under the crosshair.
    private void HandleInteraction()
    {
        targetedItem = null;
        targetedNPC = null;
        
        if (mainCam == null) return;
        
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, raycastMaxDistance, interactableMask, QueryTriggerInteraction.Collide))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, hit.collider.transform.position);
            
            if (distanceToPlayer <= interactionRange)
            {
                // Check for Item
                Item item = hit.collider.GetComponent<Item>();
                if (item != null)
                {
                    targetedItem = item;
                }
                
                NPCDialogue npc = hit.collider.GetComponent<NPCDialogue>();

                if (npc != null)
                {
                    targetedNPC = npc;
                }
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (targetedNPC != null)
            {
                DialogueNode node = targetedNPC.GetCurrentStartingNode();
                dialogueSystem.StartDialogue(targetedNPC.npcData, node);
                return;
            }
            
            if (targetedItem != null)
            {
                targetedItem.Collect(inventory);
            }
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