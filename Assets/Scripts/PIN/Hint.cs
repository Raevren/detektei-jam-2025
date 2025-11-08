using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "Hint", menuName = "Scriptable Objects/Hint")]
public class Hint : ScriptableObject
{
    [field: SerializeField] public HintStep[] hints;

    [field: SerializeField] public List<DialogSequence> fallbackDialogs;

    /// <summary>
    /// Load title from loca
    /// </summary>
    public string GetTitle => LocalizationSettings.StringDatabase.GetLocalizedString("Hints", "hint_title");
}