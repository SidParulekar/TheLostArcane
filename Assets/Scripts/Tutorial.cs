using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialScreen;

    private bool _tutorialShown = false;

    /// <summary>
    /// Show controls once player enters trigger and show it only first time they enter
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() && !_tutorialShown)
        {
            _tutorialShown = true;
            tutorialScreen.SetActive(true);
        }
    }
}
