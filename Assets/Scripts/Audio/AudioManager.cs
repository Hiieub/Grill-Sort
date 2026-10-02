using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip bgMusic;
    [SerializeField] private AudioClip clickedMusic;

    private void OnEnable()
    {
        ButtonClickSound.OnAnyButtonClicked += ButtonClickSound_OnAnyButtonClicked;
    }

    private void OnDisable()
    {
        ButtonClickSound.OnAnyButtonClicked -= ButtonClickSound_OnAnyButtonClicked;
    }

    private void ButtonClickSound_OnAnyButtonClicked()
    {
        musicSource.PlayOneShot(clickedMusic);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetSoundEnabled(IsSoundEnabled());

        PlayMusic(bgMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetSoundEnabled(bool isEnable)
    {
        if (musicSource != null) musicSource.mute = !isEnable;
        if (sfxSource != null) sfxSource.mute = !isEnable;

        PlayerPrefs.SetInt("SoundEnabled", isEnable ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsSoundEnabled()
    {
        return PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
    }
}
