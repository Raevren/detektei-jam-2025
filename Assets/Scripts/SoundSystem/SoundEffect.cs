using UnityEngine;

[CreateAssetMenu(menuName = "JAMS/SoundEffect", fileName = "SoundEffect")]
public class SoundEffect : ScriptableObject
{
    [field: SerializeField] public AudioClip Clip { get; private set; }
    [field: SerializeField] public VoiceType VoiceType { get; private set; }
}

public enum CommonSfx
{
    Submit,
    TextM,
    TextH,
    Call
}