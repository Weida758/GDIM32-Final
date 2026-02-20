using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private PlayerInputAsset playerInputAsset;
    private InputActionMap inputMap;

    private InputAction attackAction;


    [field: DisplayOnly] [field: SerializeField] public bool attacking { get; private set; }

    private void Awake()
    {
        playerInputAsset = new PlayerInputAsset();
        inputMap = playerInputAsset.Player;
    }

    private void OnEnable()
    {
        playerInputAsset.Enable();
    }
    void Start()
    {
        attackAction = inputMap.FindAction("Attack");
    }
    
    void Update()
    {
        if (attackAction.WasPressedThisFrame())
        {
            Debug.Log("Attacking");
        }
    }

    private void OnDisable()
    {
        playerInputAsset.Disable();
    }
}
