using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : PatrollingEntity, IDamageable
{

    [SerializeField] private float health = 50f;

    private float _maxHealth;

    [SerializeField] private float attackPower;
    [SerializeField] protected GameObject attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] protected float enemyAttackRange = 2f;
    [SerializeField] protected float enemyChargeRange = 5f;
     
    [SerializeField] protected float chargeSpeed;

    [SerializeField] private float pushBackDuration;
    [SerializeField] private float pushBackSpeed;

    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioClip deathSound;

    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private Color takeDamageColor;
    [SerializeField] private float damageFlashDuration;

    [SerializeField] protected GameObject collectibleSpawn;

    [SerializeField] private Image healthFillImage;

    private bool _flashDone = false;

    protected bool _attackMode = false;


    protected PlayerController _target;

    public Animator enemyAnimator;

    private float _neutralSpeed;

    protected Rigidbody2D _enemyRB;

    private bool _pushedBack = false;

    private bool _pushedBackStarted = false;

    private void Start()
    {
        base.Start();
        _maxHealth = health;
        _target = PlayerController.Instance;
        _enemyRB = GetComponent<Rigidbody2D>();

        _neutralSpeed = speed;
    }

    /// <summary>
    /// Makes enemy move when not in attack mode, pushes enemy back if it is being pushed back and switches direction
    /// </summary>
    void FixedUpdate()
    {
        if (!_attackMode)
        {
            MoveHorizontal();
            
        }

        if (_pushedBack)
        {
            PushBack();
            if (!_pushedBackStarted)
            {
                StartCoroutine(PushBackDuration());
            }           
        }

        SwitchDirection();
    }

    /// <summary>
    /// If target is within charging range thenmove towards target if within attacking range then stop moving and start attacking
    /// </summary>
    private void Update()
    {
        if (_target.IsAlive() && health>0f) 
        {
            if (WithinChargingRange())
            {
                if (WithinAttackingRange())
                {
                    enemyAnimator.Play("Attack");
                }
            }
        }

        else
        {
            SetNeutral();
        }
    }

    /// <summary>
    /// Reduces health by certain amount
    /// </summary>
    /// <param name="damage"></param>
    public void ReduceHealth(float damage)
    {
        health -= damage;
        if (!_flashDone)
        {
            StartCoroutine(DamageFlash());
        }
        if (health <= 0f)
        {
            health = 0f;
            EnemyDeath();         
        }

        UpdateHealthUI();
    }

    /// <summary>
    /// Updates the visual health bar 
    /// </summary>
    private void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = Mathf.Clamp01(health / _maxHealth);
        }

    }

    /// <summary>
    /// Flashes the enemy red when being hit
    /// </summary>
    /// <returns></returns>
    public IEnumerator DamageFlash()
    {
        _flashDone = true;
        Color orig = characterSprite.color;
        characterSprite.color = takeDamageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        characterSprite.color = orig;
        _flashDone = false;
    }

    /// <summary>
    /// Play death sound and death animation
    /// </summary>
    public virtual void EnemyDeath()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        this.enabled = false;
        SoundManager.Instance.PlaySound(deathSound);
        enemyAnimator.Play("Death");
    }

    /// <summary>
    /// Called by animation event at end of Death animation and spawns a collectible prefab
    /// </summary>
    public virtual void DestroyEnemy()
    {
        Instantiate(collectibleSpawn, transform.position, transform.rotation);  
        Destroy(gameObject);
    }

    public void PlayAttackSound()
    {
        SoundManager.Instance.PlaySound(attackSound);
    }

    /// <summary>
    /// Damages all targets that fall within the radius of the attack point game object
    /// </summary>
    public virtual void Attack()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, targetLayer);

        foreach (Collider2D target in targets)
        {
            IDamageable damageableTarget = target.GetComponent<IDamageable>();
            if (damageableTarget != null)
            {
                damageableTarget.ReduceHealth(attackPower);
            }
        }       
    }

    /// <summary>
    /// Pushes enemy back by certain speed
    /// </summary>
    public void PushBack()
    {   
        _pushedBack = true;
        _enemyRB.linearVelocityX = -1f * _direction.x * pushBackSpeed;
    }

    /// <summary>
    /// Pushback lasts for specified amount of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator PushBackDuration()
    {
        _pushedBackStarted = true;
        yield return new WaitForSeconds(pushBackDuration);
        _pushedBack = false;
        _pushedBackStarted = false;
    }

    public void endAttack()
    {                
        enemyAnimator.Play("Neutral");
    }

    /// <summary>
    /// If target is within the charging range then enemy charges towards target otherwise goes back to patrolling
    /// </summary>
    /// <returns></returns>
    public virtual bool WithinChargingRange()
    {
        Vector3 enemyScale = transform.localScale;

        Transform targetPosition = _target.transform;

        if (Mathf.Abs(targetPosition.position.x - transform.position.x) <= enemyChargeRange && Mathf.Abs(targetPosition.position.y - transform.position.y) <= 1f && _target.IsOnAttackableGround())
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

    /// <summary>
    /// If target is within attacking range then enemy stops moving and starts attacking
    /// </summary>
    /// <returns></returns>
    private bool WithinAttackingRange()
    {
        float targetPosition = _target.transform.position.x;

        if (Mathf.Abs(targetPosition - transform.position.x) <= enemyAttackRange)
        {
            _enemyRB.linearVelocityX = 0f;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets enemy back to patrolling normally
    /// </summary>
    public void SetNeutral()
    {
        enemyAnimator.SetBool("Charge", false);
        _attackMode = false;
        speed = _neutralSpeed;
    }

    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);
        }        
    }

}
