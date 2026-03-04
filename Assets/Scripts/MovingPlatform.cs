using UnityEngine;

public enum MovingPlatformAxis
{
    Vertical,
    Horizontal
}

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private MovingPlatformAxis axis;

    private float destinationPos;

    [SerializeField] private float destinationPosition;

    private Vector3 startPos;

    [SerializeField] private float speed;

    [SerializeField] private AudioClip movingSound;

    private Rigidbody2D passengerRB;

    private Rigidbody2D platformRB;

    private Vector2 direction;

    private bool reached = false;

    private bool noPassenger = false;

    private AudioSource platformAudioSource;

    private void Start()
    {
        platformRB = GetComponent<Rigidbody2D>();

        platformAudioSource = GetComponent<AudioSource>();

        startPos = transform.position;

        destinationPos = destinationPosition;

        this.enabled = false;

        SetDirection();
    }

    private void FixedUpdate()
    {
        if (axis == MovingPlatformAxis.Vertical)
        {
            VerticalMovement();           
        }

        else
        {
            HorizontalMovement();            
        }
    }

    /// <summary>
    /// Moves platform up or down depending on whether the destination y position is greater than or less than its starting poisiton
    /// </summary>
    private void VerticalMovement()
    {
        float currentPos = transform.position.y;

        if ((direction.y > 0f && currentPos >= destinationPos) || (direction.y < 0f && currentPos <= destinationPos))
        {
            this.enabled = false;
            platformRB.linearVelocityY = 0f;
            if (passengerRB != null)
            {
                passengerRB.linearVelocityY = 0f;
            }           
            reached = true;
            platformAudioSource.Stop();
            if (destinationPos != startPos.y)
            {
                ResetPlatform(startPos.y);
            }
            return;
        }
        platformRB.linearVelocityY = direction.y * speed;

        if (passengerRB != null)
        {
            passengerRB.linearVelocityY = direction.y * speed;
        }
    }

    /// <summary>
    /// Moves platform left or right depending on whether the destination x position is greater than or less than its starting poisiton
    /// </summary>
    private void HorizontalMovement()
    {
        float currentPos = transform.position.x;

        if ((direction.x > 0f && currentPos >= destinationPos) || (direction.x < 0f && currentPos <= destinationPos))
        {
            this.enabled = false;
            platformRB.linearVelocityX = 0f;           
            reached = true;
            platformAudioSource.Stop();
            if (destinationPos != startPos.x)
            {
                ResetPlatform(startPos.x);
            }
            return;
        }

        platformRB.linearVelocityX = direction.x * speed;

        if (passengerRB != null)
        {
            passengerRB.linearVelocityX = direction.x * speed;
        }
        
    }

    /// <summary>
    /// Resets platform back to starting position only once player leaves the platform
    /// </summary>
    /// <param name="destination"></param>
    private void ResetPlatform(float destination)
    {        
        destinationPos = destination;
        SetDirection();

        if (noPassenger && reached)
        {
            platformAudioSource.PlayOneShot(movingSound);
            this.enabled = true;
            passengerRB = null;
            reached = false;
            noPassenger = false;
        }
    }

    /// <summary>
    /// Sets direction of platform movement
    /// </summary>
    private void SetDirection()
    {
        if (axis == MovingPlatformAxis.Vertical)
        {
            if (destinationPos >= transform.position.y)
            {
                direction = Vector2.up;
            }

            else
            {
                direction = Vector2.down;
            }
        }

        else
        {
            if (destinationPos >= transform.position.x)
            {
                direction = Vector2.right;
            }

            else
            {
                direction = Vector2.left;
            }
        }
    }

    /// <summary>
    /// Platform starts moving only once player is on platform and resets once player leaves the platform
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            if (PlayerController.Instance.IsAlive())
            {
                platformAudioSource.PlayOneShot(movingSound);
                this.enabled = true;
                reached = false;
                passengerRB = collision.gameObject.GetComponent<Rigidbody2D>();
                destinationPos = destinationPosition;
                noPassenger = false;
                SetDirection();
            }            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            platformAudioSource.PlayOneShot(movingSound);
            this.enabled = true;
            passengerRB = null;
            reached = false;
            noPassenger = true;
        }
    }

}
