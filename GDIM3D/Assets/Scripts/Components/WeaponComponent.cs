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
    public bool isAttacking { get; private set; }
    

    private void Awake()
    {
        _currentAttackCooldown = attackCooldown;
    }

    public void Attack()
    {
        
        if (_currentAttackCooldown > 0) return;
        isAttacking = true;
        Debug.Log("I am Attacking");
        
        int hitAmount = Physics.OverlapSphereNonAlloc(hitboxOrigin.position, attackRadius,
            attackedCollidersBuffer, enemyLayer);

        for (int i = 0; i < hitAmount; i++)
        {
            HealthComponent healthComponent =
                attackedCollidersBuffer[i].GetComponent<HealthComponent>();
            if (healthComponent)
            {
                healthComponent.TakeDamage(attackPower);
            }

            _currentAttackCooldown = attackCooldown;
        }

        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hitboxOrigin.transform.position, attackRadius);
    }

    private void Update()
    {
        _currentAttackCooldown -= Time.unscaledDeltaTime;
        
    }
    
}
