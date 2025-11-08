using UnityEngine;

[CreateAssetMenu(menuName = "JAMS/MusicTrack", fileName = "MusicTrack")]
public class MusicTrack : ScriptableObject
{
    [field: SerializeField] public AudioClip Sqaure1 {get; private set;}
    [field: SerializeField] public AudioClip Sqaure2 {get; private set;}
    [field: SerializeField] public AudioClip Wave {get; private set;}
    [field: SerializeField] public AudioClip Noise {get; private set;}
}