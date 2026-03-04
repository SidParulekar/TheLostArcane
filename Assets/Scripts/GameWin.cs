using UnityEngine;

public class GameWin : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            winScreen.SetActive(true);
            PlayerController.Instance.TogglePlayerInput(false);
        }
        

    }
}
