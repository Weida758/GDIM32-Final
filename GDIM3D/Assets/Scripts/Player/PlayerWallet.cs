using UnityEngine;
using System;
public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    [SerializeField] private int startingGold = 0;

    public int Gold { get; private set; }

    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;;
        }

        Instance = this;
        Gold = startingGold;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        Gold += amount;
        Debug.Log($"PlayerWallet + {amount} gold. Total: {Gold}");
        OnGoldChanged?.Invoke(Gold);
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || Gold < amount) return false;

        Gold -= amount;
        Debug.Log($"PlayerWallet - {amount} gold. Total: {Gold}");
        OnGoldChanged?.Invoke(Gold);
        return true;
    }
}
