using System.Collections;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private float openPos;  

    [SerializeField] private float openSpeed = 3f;

    [SerializeField] private AudioClip doorOpenSound;

    private Vector2 _direction = Vector2.up;

    private Rigidbody2D _doorRB;

    private bool _unlocked = false;

    [SerializeField] TextMeshProUGUI doorPrompt;

    private void Start()
    {
        _doorRB = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_unlocked && !Opened())
        {
            OpenDoor();           
        }

        else
        {
            StopDoor();
        }
    }

    
    public void InteractResult(GameObject ob)
    {
        StartCoroutine(ShowPrompt());               
    }

    /// <summary>
    /// Shows prompt to user for 3 seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowPrompt()
    {
        doorPrompt.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        doorPrompt.gameObject.SetActive(false);
    }

    /// <summary>
    /// Unlocks door
    /// </summary>
    public void Unlock()
    {
        _unlocked = true;
        SoundManager.Instance.PlaySound(doorOpenSound);
    }

    private void OpenDoor()
    {       
        _doorRB.linearVelocityY = _direction.y * openSpeed;
    }

    private void StopDoor()
    {
        _doorRB.linearVelocityY = 0f;
    }

    /// <summary>
    /// Check if door is opened
    /// </summary>
    /// <returns></returns>
    private bool Opened()
    {
        if (transform.position.y >= openPos)
        {
            return true; 
        }

        return false;
    }
}
