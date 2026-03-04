using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance; //Created gameController singleton

    private Vector3 _checkpoint;

    [SerializeField] private float respawnPause = 2f;

    [SerializeField] private GameObject deathScreen;

    [SerializeField] private AudioClip deathMusic;

    [SerializeField] private GameObject bossCutsceneCamera;

    [SerializeField] private GameObject playerCamera;

    [SerializeField] private GameObject gameplayBackgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _checkpoint = PlayerController.Instance.transform.position;
    }

    /// <summary>
    /// Stores position of latest checkpoint
    /// </summary>
    /// <param name="currentCheckpoint"></param>
    public void SetCheckPoint(Vector3 currentCheckpoint)
    {
        _checkpoint = currentCheckpoint;
    }

    public void RespawnAtCheckpoint()
    {
        StartCoroutine(RespawnPause());
        Debug.Log("postion Changed");
    }

    /// <summary>
    /// Waits for specified amount of time after player death before respawning player at latest checkpoint position
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnPause()
    {
        yield return new WaitForSeconds(respawnPause);
        PlayerController.Instance.gameObject.transform.position = _checkpoint;
        PlayerController.Instance.Respawn();
       
    }

    /// <summary>
    /// Reloads game scene
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads the Lobby scene
    /// </summary>
    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Displays death screen popup
    /// </summary>
    public void DisplayDeathScreen()
    {
        SoundManager.Instance.PlaySound(deathMusic);
        deathScreen.SetActive(true);
    }

    /// <summary>
    /// Activates the camera for boss entry cutscene and deactivates player camera, input and regular background music
    /// </summary>
    public void ActivateBossEntry()
    {
        playerCamera.SetActive(false);
        bossCutsceneCamera.SetActive(true);
        gameplayBackgroundMusic.SetActive(false);
        PlayerController.Instance.TogglePlayerInput(false);
    }

    /// <summary>
    /// Resets player camera and input after cutscene is over
    /// </summary>
    public void ResetPlayerCamera()
    {
        playerCamera.SetActive(true);
        bossCutsceneCamera.SetActive(false);
        PlayerController.Instance.TogglePlayerInput(true);
    }

    public void RestartGameplayBackgroundMusic()
    {
        gameplayBackgroundMusic.SetActive(true);
    }
}
