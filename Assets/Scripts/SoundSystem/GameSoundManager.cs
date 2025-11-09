using UnityEngine;
using UnityEngine.Serialization;

public class GameSoundManager : MonoBehaviour
{
    private static GameSoundManager _instance;
    [SerializeField] private MusicTrack defaultTrack;
    private const string BGMKey = "CurrentTownMusic";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        LoadTownBGM();
    }

    /// <summary>
    /// Plays the currently set town music
    /// </summary>
    public static void PlayCurrentBgm()
    {
        if(_instance == null) Debug.LogWarning("[GameSoundManager] BGM could not be played: GameSoundManager is not present in scene!");
        _instance?.LoadTownBGM();
    }

    /// <summary>
    /// Loads the currently set town music
    /// </summary>
    private void LoadTownBGM()
    {
        SoundSystem.Instance.PlayMusic(PlayerPrefs.HasKey(BGMKey)
            ? Resources.Load<MusicTrack>("BGM/" + PlayerPrefs.GetString(BGMKey)) ?? defaultTrack
            : defaultTrack);
    }

    /// <summary>
    /// Set the new default town theme
    /// </summary>
    public static void SwitchTrack(MusicTrack track)
    {
        if(_instance == null) Debug.LogWarning("[GameSoundManager] New BGM could not be set: GameSoundManager is not present in scene!");
        _instance?.SetNewTownMusic(track);
    }

    private void SetNewTownMusic(MusicTrack track)
    {
        SoundSystem.Instance.PlayMusic(track);
        PlayerPrefs.SetString(BGMKey, track.name);
    }
}
