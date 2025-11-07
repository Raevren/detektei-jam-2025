using System;
using UnityEngine;

public class JamDebug : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField] private DialogSequence testDialog;
    
    private void Start()
    {
        DialogCanvas.Spawn(testDialog);
    }

#endif
}
