using UnityEngine;
using System;
public class HealthComponent : MonoBehaviour
{
    [field: SerializeField] public float maxHealth { get; private set; }
    [field: SerializeField] public float currentHealth { get; private set; }

    public event Action OnHealthChanged;


    private void Start()
    {
        currentHealth = maxHealth;
    }

    [ContextMenu("Take 20 Damage")]
    public void Take20Damage()
    {
        currentHealth -= 20;
        OnHealthChanged?.Invoke();
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        OnHealthChanged?.Invoke();
    }
    
}
