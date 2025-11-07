using System;
using UnityEngine;

[Serializable]
public class SingleDialog
{
    [field: SerializeField] public string ActorSpriteSuffix { get; private set; }
    [field: SerializeField] public string DialogKey { get; private set; }
}