using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "HintStep", menuName = "Scriptable Objects/HintStep")]
public class HintStep : ScriptableObject
{
    [field: SerializeField] public DialogSequence CompletedDialog { get; private set; }
    [field: SerializeField] public Hint[] HintsToUnlock { get; private set; }
    
    [field: SerializeField] public Hint[] NeededConnectedHints { get; private set; }
    [field: SerializeField] public Hint[] AlternativeConnectedHints { get; private set; }
    
    [field: SerializeField] public Sprite HintImage { get; private set; }
    
    /// <summary>
    /// Load description from loca
    /// </summary>
    public string GetDescription => LocalizationSettings.StringDatabase.GetLocalizedString("Hints", name);
}
