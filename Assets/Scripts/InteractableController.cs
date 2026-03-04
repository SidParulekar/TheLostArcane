using UnityEngine;

public class InteractableController : MonoBehaviour
{
    /// <summary>
    /// Gets reference of script that implements IInteractable interface and calls the interface function
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            IInteractable interactable = GetComponent<IInteractable>();
            interactable.InteractResult(collision.gameObject);
        }
    }
}
