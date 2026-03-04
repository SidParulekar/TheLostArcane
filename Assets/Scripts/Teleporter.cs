using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable
{
    [SerializeField] Transform destination;

    [SerializeField] private AudioClip teleportSound;

    /// <summary>
    /// Set position of player to specified destination when player enters teleporter trigger
    /// </summary>
    /// <param name="ob"></param>
    public void InteractResult(GameObject ob)
    {
        SoundManager.Instance.PlaySound(teleportSound);
        ob.transform.position = destination.position;
    }
}
