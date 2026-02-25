using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private PlayerInputAsset playerInputAsset;
    private InputActionMap inputMap;

    private InputAction attackAction;
    private InputAction inventoryAction;
    private InputAction jumpAction;


    [field: DisplayOnly] [field: SerializeField] public bool attacking { get; private set; }

    [field: DisplayOnly]
    [field: SerializeField]
    public bool inventoryPressed { get; private set; }
    public bool jumpButtonPressed { get; private set; }
    

    private void Awake()
    {
        playerInputAsset = new PlayerInputAsset();
        inputMap = playerInputAsset.Player;
        
        
        attackAction = inputMap.FindAction("Attack");
        inventoryAction = inputMap.FindAction("Inventory");
        jumpAction = inputMap.FindAction("Jump");
    }

    private void OnEnable()
    {
        playerInputAsset.Enable();
    }
    
    void Update()
    {
        attacking = attackAction.WasPressedThisFrame();
        inventoryPressed = inventoryAction.WasPressedThisFrame();
        jumpButtonPressed = jumpAction.WasPressedThisFrame();
    }

    private void OnDisable()
    {
        playerInputAsset.Disable();
    }
}
