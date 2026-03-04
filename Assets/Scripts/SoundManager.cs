using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource _source;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        _source.PlayOneShot(clip);
    }

    public void StopSound()
    {
        _source.Stop();
    }

}
