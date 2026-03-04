using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private float damage;

    /// <summary>
    /// Cause player damage when in contact with spikes
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            Damage(collision.gameObject.GetComponent<PlayerController>());
            SoundManager.Instance.PlaySound(impactSound);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            Damage(collision.gameObject.GetComponent<PlayerController>());
        }
    }

    private void Damage(PlayerController player)
    {       
        player.ReduceHealth(damage);              
    }
    
}
