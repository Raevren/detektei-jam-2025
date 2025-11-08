using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "HintStep", menuName = "Scriptable Objects/HintStep")]
public class HintStep : ScriptableObject
{
    [SerializeField] private string key;
    
    /// <summary>
    /// Load description from loca
    /// </summary>
    public string GetDescription => LocalizationSettings.StringDatabase.GetLocalizedString("Hints", key);
}
