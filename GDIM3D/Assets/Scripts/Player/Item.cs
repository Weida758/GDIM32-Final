using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public int amount;
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private float floatingSpeed;
    [SerializeField] private float floatingRangeUp;
    [SerializeField] private float floatingRangeDown;

    private SpriteRenderer spriteRenderer;
    private PlayerInventory inventory;
    private Vector3 originalPosition;
    private Vector3 rotation = Vector3.zero;
    private bool playerWithinRange;



    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.itemSprite;
        }
    }

    private void Start()
    {
        originalPosition = transform.position;
    }
    private void Update()
    {
        if (playerWithinRange && inventory)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Picking up item");
                if (inventory.TryAddItem(itemData, amount))
                {
                    Destroy(gameObject);
                }
            }
        }
        rotation += new Vector3(0f,  rotatingSpeed * Time.deltaTime, 0f);
        transform.rotation = Quaternion.Euler(rotation);
        if (transform.position.y > originalPosition.y + floatingRangeUp || 
            transform.position.y < originalPosition.y - floatingRangeDown)
        {
            floatingSpeed = -floatingSpeed;
        }
        
        transform.Translate(Vector3.up * floatingSpeed * Time.deltaTime);
        
    }


    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.CompareTag("Player")) return;

        Debug.Log("Entering object pick up range");
        if (inventory == null)
        {
            inventory = coll.GetComponent<PlayerInventory>();
        }

        playerWithinRange = true;

    }
    
    
    

    private void OnTriggerExit(Collider coll)
    {
        if (!coll.CompareTag("Player")) return;
        playerWithinRange = false;
    }
    
    public bool Collect(PlayerInventory playerInventory)
    {
        if (playerInventory == null) return false;

        if (playerInventory.TryAddItem(itemData, amount))
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }


}