using UnityEngine;

public class PatrollingEntity : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected Vector2 startDirection; //To specify whether patrolling entity will be horizontal or vertical
    protected Vector2 _direction;

    private float _startPos;
    private float _endPos;

    [SerializeField] protected float patrollingRange;

    Rigidbody2D patrollingRB;

    /// <summary>
    /// Sets the patrolling area of the object based on the specified patrolling range
    /// </summary>
    protected virtual void Start()
    {
        patrollingRB = GetComponent<Rigidbody2D>();

        _direction = startDirection;

        if (_direction.x != 0f) //If horizontal patrolling entity
        {
            _startPos = transform.position.x - patrollingRange;
            _endPos = transform.position.x + patrollingRange;
        }

        else if(_direction.y != 0f) // If vertical patrolling entity
        {
            _startPos = transform.position.y - patrollingRange;
            _endPos = transform.position.y + patrollingRange;
        }
        
    }   

    /// <summary>
    /// Object moves horizontally within the patrolling range
    /// </summary>
    public void MoveHorizontal() 
    {
        Vector3 position = transform.position;

        if (position.x <= _startPos)
        {
            _direction = Vector2.right;
        }

        else if (position.x >= _endPos)
        {
            _direction = Vector2.left;
        }

        patrollingRB.linearVelocityX = _direction.x * speed;

    }

    /// <summary>
    /// Object moves vertically within the patrolling range
    /// </summary>
    public void MoveVertical()
    {
        Vector3 position = transform.position;

        if (position.y <= _startPos)
        {
            _direction = Vector2.up;
        }

        else if (position.y >= _endPos)
        {
            _direction = Vector2.down;
        }

        patrollingRB.linearVelocityY = _direction.y * speed;
    }

    /// <summary>
    /// Switches the direction the object faces
    /// </summary>
    public virtual void SwitchDirection()
    {
        Vector3 scale = transform.localScale;
        if (_direction == Vector2.left)
        {
            scale.x = -1f * Mathf.Abs(scale.x);
        }

        else if (_direction == Vector2.right)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    
}
