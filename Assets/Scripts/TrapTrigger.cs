using System.Collections;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private GameObject trap;

    [SerializeField] private Sprite activatedSprite;

    private SpriteRenderer _trapTriggerSprite;

    private bool _trapTriggered = false;    

    private void Start()
    {
        _trapTriggerSprite = GetComponent<SpriteRenderer>();
    }
    
    /// <summary>
    /// If player enters trap trigger change the sprite and activate trap
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() && !_trapTriggered)
        {
            trap.SetActive(true);
            _trapTriggerSprite.sprite = activatedSprite;
            _trapTriggered = true;
        }
    }

    /// <summary>
    /// When player leaves trap trigger despawn the trap if its not a patrolling trap
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            if (!trap.GetComponentInChildren<PatrollingEntity>())
            {
                if (trap.activeInHierarchy)
                {
                    StartCoroutine(DisableDamageAfter());
                    StartCoroutine(TrapDespawnInterval());
                }              
            }
        }
    }

    /// <summary>
    /// Despawn trap after certain amount of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator TrapDespawnInterval()
    {
        yield return new WaitForSeconds(4f);
        trap.SetActive(false);
    }

    /// <summary>
    /// Disable the damage of trap after certain amount of time. This is to prevent player from walking over an already triggered trap and getting damage
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableDamageAfter()
    {
        yield return new WaitForSeconds(1f);
        SetTagForAllChildren(trap, "Untagged");
    }

    private void SetTagForAllChildren(GameObject parent, string tag)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            child.gameObject.tag = tag;
        }
    }
}
