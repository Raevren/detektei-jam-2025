using System.Collections;
using System.Threading;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    public static SoundSystem Instance { get; private set; }
    
    private MusicTrack _currentMusic;
    
    [Header("Voices")] 
    [SerializeField] private AudioSource square1;
    [SerializeField] private AudioSource square2;
    [SerializeField] private AudioSource wave;
    [SerializeField] private AudioSource noise;

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

    /// <summary>
    /// Instantly stops the music
    /// </summary>
    public void StopMusic()
    {
        square1.Stop();
        square2.Stop();
        wave.Stop();
        noise.Stop();
        _currentMusic = null;
    }

    /// <summary>
    /// Play a new piece of music
    /// </summary>
    public void PlayMusic(MusicTrack music)
    {
        if(_currentMusic != null) StopMusic();
        _currentMusic = music;
        
        // Set the tracks
        square1.clip = music.Sqaure1;
        square2.clip = music.Sqaure2;
        wave.clip = music.Wave;
        noise.clip = music.Noise;
        
        // Play the tracks
        square1.Play();
        square2.Play();
        wave.Play();
        noise.Play();
    }

    public void PlaySfx(SoundEffect sfx)
    {
        AudioSource sourceToMute = sfx.VoiceType switch
        {
            VoiceType.Square => square2.mute ? square1 : square2,
            VoiceType.Wave => wave,
            _ => noise
        };

        AudioSource soundSource = Instantiate(Resources.Load<AudioSource>("SoundEffect"));
        DontDestroyOnLoad(soundSource.gameObject);
        StartCoroutine(SfxLifetime(soundSource, sfx, sourceToMute));
    }

    private IEnumerator SfxLifetime(AudioSource source, SoundEffect sfx, AudioSource sourceToMute)
    {
        source.clip = sfx.Clip;
        source.Play();
        sourceToMute.mute = true;
        yield return new WaitForSeconds(sfx.Clip.length);
        sourceToMute.mute = false;
        Destroy(source.gameObject);
    }
    
    public void PlayGenericSfx(CommonSfx which)
    {
        SoundEffect sfx = Resources.Load<SoundEffect>("SFX/" + which);
        if (sfx != null) PlaySfx(sfx);
    }
}

public enum VoiceType
{
    Square,
    Wave,
    Noise
}