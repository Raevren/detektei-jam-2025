using UnityEngine;

[CreateAssetMenu(fileName = "Hint", menuName = "Scriptable Objects/Hint")]
public class Hint : ScriptableObject
{
    [TextArea(3, 10)]
    public string hintTitle;
    
    public HintStep[] hints;
}
