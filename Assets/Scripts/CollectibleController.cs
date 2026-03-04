using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private Item item;

    [SerializeField] private AudioClip pickUpSound;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            SoundManager.Instance.PlaySound(pickUpSound);
            Destroy(gameObject);
        }
    }

    public Item GetItem()
    {
        return item;
    }
}
