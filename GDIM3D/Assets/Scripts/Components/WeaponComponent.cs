using UnityEngine;
using UnityEngine.Serialization;

public class WeaponComponent : MonoBehaviour
{
    [field: SerializeField] public float attackPower { get; private set; }
    [field: SerializeField] public float attackCooldown { get; private set; }
    private float _currentAttackCooldown;
    [SerializeField] private Transform hitboxOrigin;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemyLayer;
    private Collider[] attackedCollidersBuffer = new Collider[20];
    

    private void Awake()
    {
        _currentAttackCooldown = attackCooldown;
    }

    public void Attack(float value)
    {

        if (_currentAttackCooldown > 0) return;
        
        int hitAmount = Physics.OverlapSphereNonAlloc(hitboxOrigin.position, attackRadius,
            attackedCollidersBuffer, enemyLayer);

        for (int i = 0; i < hitAmount; i++)
        {
            HealthComponent healthComponent =
                attackedCollidersBuffer[i].GetComponent<HealthComponent>();
            if (healthComponent)
            {
                healthComponent.TakeDamage(value);
            }

            _currentAttackCooldown = attackCooldown;
        }
    }

    private void Update()
    {
        _currentAttackCooldown -= Time.unscaledDeltaTime;
        
    }
    
}
