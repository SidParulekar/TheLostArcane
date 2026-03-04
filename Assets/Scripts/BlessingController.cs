using UnityEngine;

public class BlessingController : MonoBehaviour
{
    [SerializeField] private AudioClip pickUpSound;

    /// <summary>
    ///Generated a random integer for index of player upgrade (for a random upgrade when game object gets picked up) 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            SoundManager.Instance.PlaySound(pickUpSound);
            int upgrade = Random.Range(1, 4);
            PlayerController.Instance.BlessingUpgrade(upgrade);
            Destroy(gameObject);
        }
    }
}
