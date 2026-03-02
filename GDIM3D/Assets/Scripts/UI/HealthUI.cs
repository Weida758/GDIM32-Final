using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private UnityEngine.UI.Image healthBar;
    
    private void Start()
    {
        UpdateHealth();
    }
    private void OnEnable()
    {
        Locator.instance.player.healthComponent.OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        Locator.instance.player.healthComponent.OnHealthChanged -= UpdateHealth;
    }

    private void UpdateHealth()
    {
        var healthComponent = Locator.instance.player.healthComponent;
        healthText.text = $"{healthComponent.currentHealth}/{healthComponent.maxHealth}";
        healthBar.fillAmount = healthComponent.currentHealth / healthComponent.maxHealth;
    }
}
