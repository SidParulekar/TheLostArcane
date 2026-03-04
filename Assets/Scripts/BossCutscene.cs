using UnityEngine;

public class BossCutscene : MonoBehaviour
{
    [SerializeField] GameObject boss;

    [SerializeField] GameObject bossDoor;

    /// <summary>
    /// Sets the curscene game objects to false and the actual boss enemy to true
    /// </summary>
    private void EndCutscene()
    {
        GameController.Instance.ResetPlayerCamera();

        boss.SetActive(true);

        bossDoor.SetActive(false);

        gameObject.SetActive(false);
    }
}
