using UnityEngine;

public class Locator : MonoBehaviour
{
    public static Locator instance;

    [field: SerializeField] public Player player { get; private set; }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    
    
}
