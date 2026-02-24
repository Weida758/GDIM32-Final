using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private HealthComponent healthComponent;

    private void Start()
    {
        UpdateHealth();
    }
    private void OnEnable()
    {
        healthComponent.OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        healthComponent.OnHealthChanged -= UpdateHealth;
    }

    private void UpdateHealth()
    {
        healthText.text = $"{healthComponent.currentHealth}/{healthComponent.maxHealth}";
    }
}
