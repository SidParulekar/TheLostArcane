using UnityEngine;

public class BossController : SpitterController
{

    [SerializeField] private GameObject bossFightBackgroundMusic;

    [SerializeField] private GameObject bossDefeatMusic;

    [SerializeField] private float pushBackDamage;

    //The original facing direction of boss sprite is opposite direction so had to change the switch direction logic to fit that
    public override void SwitchDirection()
    {
        Vector3 scale = transform.localScale;
        if (_direction == Vector2.left)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        else if (_direction == Vector2.right)
        {
            scale.x = -1f * Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    /// <summary>
    /// Changed the difference in y pos between player and boss required to stop charging
    /// </summary>
    /// <returns></returns>
    public override bool WithinChargingRange()
    {
        Vector3 enemyScale = transform.localScale;

        Transform targetPosition = _target.transform;

        if (Mathf.Abs(targetPosition.position.x - transform.position.x) <= enemyChargeRange && Mathf.Abs(targetPosition.position.y - transform.position.y) <= 3f && _target.IsOnAttackableGround())
        {
            enemyAnimator.SetBool("Charge", true);
            _attackMode = true;
            speed = chargeSpeed;

            if (targetPosition.position.x > transform.position.x)
            {
                _direction = Vector2.right;
            }

            else
            {
                _direction = Vector2.left;
            }

            SwitchDirection();
            _enemyRB.linearVelocityX = _direction.x * speed;

            return true;
        }

        else
        {
            SetNeutral();
        }

        transform.localScale = enemyScale;

        return false;
    }

    public override void EnemyDeath()
    {
        bossFightBackgroundMusic.SetActive(false);
        bossDefeatMusic.SetActive(true);
        gameObject.layer = LayerMask.NameToLayer("Default");
        this.enabled = false;
        SoundManager.Instance.PlaySound(deathSound);
        enemyAnimator.Play("Death");
    }

    public override void DestroyEnemy()
    {
        GameController.Instance.RestartGameplayBackgroundMusic();
        Instantiate(collectibleSpawn, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    /// <summary>
    /// Pushes back player when it comes in contact with boss
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _target.PushBack();
        _target.ReduceHealth(pushBackDamage);
    }

}
