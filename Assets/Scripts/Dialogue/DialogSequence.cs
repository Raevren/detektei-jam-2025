using System;
using System.Collections.Generic;
using UnityEngine;

// !! JAMS is short for Johanna Ascal Mico Selina !!
[CreateAssetMenu(menuName = "JAMS/Dialog", fileName = "DialogCanvas")]
public class DialogSequence : ScriptableObject
{
    // Every line, in order
    [field: SerializeField] public List<CharacterDialog> Dialogs { get; private set; }
    
    // If set, this will be the new town BGM until overridden
    [field: SerializeField] public MusicTrack PlayOnEnd { get; private set; }
}

[Serializable]
public class CharacterDialog
{
    [field: SerializeField] public DialogActor Actor { get; private set; }
    
    // Translation keys
    [field: SerializeField] public List<SingleDialog> Lines { get; private set; }
}

public enum DialogActor
{
    DetectiveMiez,
    henryHabicht,
    missMiau,
    brunoBör,
    karlNikel,
    kurtKroko,
    pfefferPig,
    pherdiPhuchs,
    professorBello,
    zoeZiege,
    telephone,
    peterPiep
}