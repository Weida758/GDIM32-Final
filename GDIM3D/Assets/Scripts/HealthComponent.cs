using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [field: SerializeField] public float maxHealth { get; private set; }
    [field: SerializeField] public float currentHealth { get; private set; }


    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
    }
    
}
