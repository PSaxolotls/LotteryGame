using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioSource _source;

    public AudioClip commonSound;
    public AudioClip tickTimer;
    public AudioClip noMoreBets;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure we have an AudioSource
        _source = GetComponent<AudioSource>();
        if (_source == null)
            _source = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            _source.PlayOneShot(clip);
    }

    public void PlayCommonSound()
    {
        if (commonSound != null)
            _source.PlayOneShot(commonSound);
    }
}
