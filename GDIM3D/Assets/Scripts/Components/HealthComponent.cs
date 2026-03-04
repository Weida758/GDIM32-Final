using UnityEngine;
using System;
using System.Collections;

public class HealthComponent : MonoBehaviour
{
    [field: SerializeField] public float maxHealth { get; private set; }
    [field: SerializeField] public float currentHealth { get; private set; }

    public event Action OnHealthChanged;
    

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
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke();
    }

    public void ContinuousHeal(float amountPerTick, float timePerTick, float duration)
    {
        StartCoroutine(HealOvertimeCoroutine(amountPerTick, timePerTick, duration));
    }

    private IEnumerator HealOvertimeCoroutine(float amountPerTick, float timePerTick, float duration)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            Heal(amountPerTick);
            timePassed += timePerTick;
            Debug.Log(timePassed);
            yield return new WaitForSeconds(timePerTick);
            
        }
        
    }
    
    
}
