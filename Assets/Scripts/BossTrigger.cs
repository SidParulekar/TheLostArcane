using UnityEngine;

public class BossTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject bossDoor;
    [SerializeField] GameObject boss;

    [SerializeField] GameObject bossDoorObject;
    
    /// <summary>
    /// Starts the boss entry cutscene when player collides with game object
    /// </summary>
    /// <param name="player"></param>
    public void InteractResult(GameObject player)
    {
        bossDoor.SetActive(true);
        boss.SetActive(true);
        bossDoorObject.SetActive(false);
        GameController.Instance.ActivateBossEntry();
        gameObject.SetActive(false);
    }
}
