using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "Hint", menuName = "Scriptable Objects/Hint")]
public class Hint : ScriptableObject
{
    [SerializeField] private string titleKey;
    
    [field: SerializeField] public HintStep[] hints;
    
    /// <summary>
    /// Load title from loca
    /// </summary>
    public string GetTitle => LocalizationSettings.StringDatabase.GetLocalizedString("Hints", titleKey);
}
