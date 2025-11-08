using System;
using UnityEngine;

public class JamDebug : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField] private DialogSequence testDialog;
    [SerializeField] private MusicTrack testMusic;
    
    private void Start()
    {
        DialogCanvas.Spawn(testDialog);
        SoundSystem.Instance.PlayMusic(testMusic);
    }

#endif
}
