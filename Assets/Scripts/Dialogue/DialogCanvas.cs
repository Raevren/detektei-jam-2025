using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.U2D;
using UnityEngine.UI;

public class DialogCanvas : MonoBehaviour
{
    private static DialogCanvas _instance;

    [SerializeField] private SpriteAtlas portraitAtlas;
    [SerializeField] private Image actorImage;
    [SerializeField] private TextMeshProUGUI nameText, dialogText;
    [SerializeField] private Button advanceButton;
    private Animator _anim;

    private DialogSequence _sequence;
    private CharacterDialog _currentDialog;
    private bool _isTalking;
    
    /// <summary>
    /// The progress of the current character's dialog
    /// </summary>
    private int _boxOfCurrentDialog = 0;
    
    /// <summary>
    /// The overall progress in the sequence
    /// </summary>
    private int _sequenceProgress = 0;
    
    private Action _onEnd;
    
    /// <summary>
    /// Use this to spawn a new dialog on screen.
    /// Any existing dialog will end.
    /// </summary>
    public static void Spawn(DialogSequence sequence, Action onEnd = null)
    {
        DialogCanvas prefab = Resources.Load<DialogCanvas>("DialogCanvas");
        DialogCanvas canvas = Instantiate(prefab);
        canvas.Setup(sequence, onEnd);
    }

    private void Setup(DialogSequence sequence, Action onEnd)
    {
        if (_instance != null)
        {
            _instance.End(true);
        }
        _instance = this;
        _anim = GetComponent<Animator>();
        GetComponent<Canvas>().worldCamera = Camera.main;
        _sequence = sequence;
        _currentDialog = sequence.Dialogs[0];
        dialogText.text = "";
        
        advanceButton.onClick.AddListener(OnClickAdvance);
    }

    /// <summary>
    /// Animates the current line
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReadText()
    {
        _isTalking = true;
        while (dialogText.maxVisibleCharacters < dialogText.text.Length)
        {
            dialogText.maxVisibleCharacters += 3;
            yield return new WaitForSeconds(0.05f);
        }
        SkipText();
    }
    
    /// <summary>
    /// End the current text animation
    /// </summary>
    private void SkipText()
    {
        StopAllCoroutines();
        dialogText.maxVisibleCharacters = dialogText.text.Length;
        _isTalking = false;
    }

    /// <summary>
    /// Gets called when the user clicks on the advance button
    /// </summary>
    private void OnClickAdvance()
    {
        if(_isTalking)
            SkipText();
        else
            Advance();
    }

    /// <summary>
    /// End the current text box and either continue or end the dialog sequence
    /// </summary>
    private void Advance()
    {
        _boxOfCurrentDialog++;
        if(_boxOfCurrentDialog >= _currentDialog.Lines.Count)
            End(false);
        else 
            GoToNext();
    }
    
    /// <summary>
    /// Just display the next dialog in this sequence
    /// </summary>
    private void GoToNext()
    {
        _anim.SetBool("HasNext", true);
        // Manually start the next box
        Anim_LoadText();
        Anim_StartText();
    }

    /// <summary>
    /// End the current dialog and fade out
    /// </summary>
    public void End(bool instantly)
    {
        StopAllCoroutines();
        _isTalking = false;
        _sequenceProgress++;
        if (instantly)
        {
            // End now!!!
            _anim.SetBool("HasNext", false);
            _anim.SetTrigger("End");
            return;
        }
        // End slowly...
        _anim.SetBool("HasNext", _sequenceProgress < _sequence.Dialogs.Count);
        _anim.SetTrigger("GoNext");
        
        if (_sequenceProgress >= _sequence.Dialogs.Count)
        {
            // End
            _instance = null;
            return;
        }
        // Load the next dialog
        _boxOfCurrentDialog = 0;
        _currentDialog = _sequence.Dialogs[_sequenceProgress];
    }

    // Called by animator when the text is supposed to start
    private void Anim_StartText()
    {
        StartCoroutine(ReadText());
    }
    
    // Called by animator when everything is faded out
    private void Anim_LoadText()
    {
        // Load the character's name
        string characterName = LocalizationSettings.StringDatabase.GetLocalizedString("Characters", _currentDialog.Actor.ToString());
        nameText.text = characterName;
        
        // Load the next line
        dialogText.maxVisibleCharacters = 0;
        dialogText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Dialog", _currentDialog.Lines[_boxOfCurrentDialog].DialogKey);
        
        // Load the actor image
        string spriteName = _currentDialog.Lines[_boxOfCurrentDialog].ActorSpriteSuffix.Trim();
        actorImage.sprite = portraitAtlas.GetSprite(_currentDialog.Actor + (string.IsNullOrEmpty(spriteName) ? "" : "_" + spriteName));
    }
    
    // Called by animator when every dialog is over
    private void Anim_Destroy()
    {
        _onEnd?.Invoke();
        Destroy(gameObject);
    }
    
}