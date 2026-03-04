using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject checkpointPopup;

    private bool _checkpointReached = false;

    /// <summary>
    /// Pops up a message for player to read when player enters trigger and sets respawn position of player to position of checkpoint game object
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() && !_checkpointReached)
        {
            GameController.Instance.SetCheckPoint(transform.position);
            checkpointPopup.SetActive(true);
            _checkpointReached = true;
        }
    }

    /// <summary>
    /// Makes sure the popup message only appears first time the player enters checkpoint
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {            
            checkpointPopup.SetActive(false);
        }
    }
}
