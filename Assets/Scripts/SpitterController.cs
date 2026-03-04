using UnityEngine;

public class SpitterController : EnemyController
{
    [SerializeField] private GameObject projectilePrefab;

    private GameObject _projectile;

    private Projectile _projectileController;
    
    /// <summary>
    /// Called by animation event in Spitter attack animation. Instantiates the projectile prefab and sets its direction and max range
    /// </summary>
    public override void Attack()
    {
        _projectile = Instantiate(projectilePrefab, attackPoint.transform.position, attackPoint.transform.rotation);

        if (_projectile.GetComponent<Projectile>())
        {
            _projectileController = _projectile.GetComponent<Projectile>();
            _projectileController.SetDirection(_direction.x);
            _projectileController.SetDestroyProjectileRange(enemyAttackRange * 2f);
        }
        
    }

}
