using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Call : MonoBehaviour
{
    [SerializeField] private Button callButton;
 
    private HintManager _hintManager;
    
    private void Start()
    {
        _hintManager = GetComponent<HintManager>();
        
        callButton.onClick.AddListener(OnPressCall);
    }
    
    private void OnPressCall()
    {
        if (_hintManager.CurrentHint == null) return;
        SoundSystem.Instance.PlayGenericSfx(CommonSfx.Call);
        
        if (!_hintManager.IsHintStepCompleted(_hintManager.CurrentHint, _hintManager.CurrentHintStep))
        {
            // Show fallback dialog
            if (_hintManager.CurrentHint.fallbackDialogs.Count == 0) return;
            DialogSequence fallbackDialog = _hintManager.CurrentHint.fallbackDialogs[Random.Range(0, _hintManager.CurrentHint.fallbackDialogs.Count)];
            DialogCanvas.Spawn(fallbackDialog);
            return;
        }
        
        // The player has completed the step
        if(_hintManager.CurrentHintStep.CompletedDialog != null)
            DialogCanvas.Spawn(_hintManager.CurrentHintStep.CompletedDialog, () => _hintManager.OnStepCompleted(_hintManager.CurrentHintStep));
        else
            DialogCanvas.Spawn(_hintManager.CurrentHint.fallbackDialogs[0]);
    }
}
