using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage;

    [SerializeField] private float speed;

    private Rigidbody2D _projectileRB;

    private Vector2 _direction;

    private float _destroyRange = 1f;

    private float _startPos;

    private void Start()
    {        
        _projectileRB = GetComponent<Rigidbody2D>();
        _startPos = transform.position.x;

        Debug.Assert(_destroyRange >= 0f);
    }

    private void Update()
    {
        float currentPos = transform.position.x;
        if (Mathf.Abs(currentPos - _startPos) >= _destroyRange) //Destroy projectile after travelling a ceratin distance
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(float shootDirection)
    {
        _direction.x = shootDirection;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())//Cause damage to target and destroy projectile on hit
        {
            PlayerController.Instance.ReduceHealth(damage);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        ShootProjectile();
    }

    private void ShootProjectile()
    {
        _projectileRB.linearVelocityX = _direction.x * speed;
    }

    public void SetDestroyProjectileRange(float range) //Set max range projectile can travel
    {
        _destroyRange = range;
    }

}
