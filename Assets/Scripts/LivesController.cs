using System.Collections.Generic;
using UnityEngine;

public class LivesController : MonoBehaviour
{
    public static LivesController Instance { get; private set; }

    private List<GameObject> lifeHearts = new List<GameObject>();

    [SerializeField] private GameObject lifeHeart;

    [SerializeField] private int lives;

    [SerializeField] private RectTransform livesGrid;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Instantiates life heart prefab in the grid object which has been configured to display horizontally
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < lives; i++)
        {
            GameObject life = Instantiate(lifeHeart, livesGrid);
            lifeHearts.Add(life);
        }
    }

    /// <summary>
    /// Destroys the life heart prefab and if no lives left then displays death screen
    /// </summary>
    /// <returns></returns>
    public bool DecreaseLife()
    {
        --lives;
        Destroy(lifeHearts[lifeHearts.Count - 1]);
        lifeHearts.RemoveAt(lifeHearts.Count - 1);
        if (lives <= 0)
        {            
            GameController.Instance.DisplayDeathScreen();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds life and life heart prefab
    /// </summary>
    public void IncreaseLife()
    {
        ++lives;
        GameObject life = Instantiate(lifeHeart, livesGrid);
        lifeHearts.Add(life);
    }

}
