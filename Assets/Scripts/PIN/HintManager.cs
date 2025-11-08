using System;
using System.Collections.Generic;
using System.Linq;
using PIN;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _title;

    [SerializeField]
    private TMP_Text _description;

    [SerializeField] 
    private Image _image;

    [SerializeField]
    private UILineRenderer _lineRenderer;
    
    [SerializeField]
    private UIDragConnector[] _connectors;

    [SerializeField] private DialogSequence startDialog;
    [SerializeField] private Hint[] startHints;
    
    private RectTransform _rectTransform;
 
    private List<string> _unlockedHints = new();
    private List<string> _hintConnectionsKeys = new();
    
    public Hint CurrentHint { get; private set; }
    public HintStep CurrentHintStep { get; private set; }

    private void Init()
    {
        _unlockedHints = new List<string>(PlayerPrefs.GetString("hint_unlocked", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        
        foreach (var uiDragConnector in _connectors)
        {
            var isActive = _unlockedHints.Contains(uiDragConnector.Hint.name);
            uiDragConnector.gameObject.SetActive(isActive);
        }
        
        _rectTransform = _lineRenderer.gameObject.GetComponent<RectTransform>();
       
        var raw = PlayerPrefs.GetString("hint_connections", "");
        Debug.Log($"[HintManager] Start: raw saved hint_connections='{raw}'");
        _hintConnectionsKeys = new List<string>(raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

        Debug.Log($"[HintManager] Start: rectTransform={( _rectTransform != null ? "ok" : "null")}, loaded { _hintConnectionsKeys.Count} connection keys.");
        if (_hintConnectionsKeys.Count > 0)
        {
            Debug.Log($"[HintManager] Loaded hint connections: {string.Join(",", _hintConnectionsKeys)}");
        }
        
        foreach (var key in _hintConnectionsKeys)
        {
            // Split at last underscore to allow hint names that may contain underscores
            var sepIndex = key.LastIndexOf('_');
            if (sepIndex <= 0 || sepIndex >= key.Length - 1)
            {
                Debug.LogWarning($"[HintManager] Start: invalid connection key format '{key}', expected 'nameA_nameB'");
                continue;
            }

            var nameA = key.Substring(0, sepIndex);
            var nameB = key.Substring(sepIndex + 1);

            var connectorA = FindConnectorByHintName(nameA);
            var connectorB = FindConnectorByHintName(nameB);

            var rectA = connectorA.GetComponent<RectTransform>();
            var rectB = connectorB.GetComponent<RectTransform>();

            // refresh cached rect transform reference in case EnsureLineRendererRect modified parent/anchors
            _rectTransform = _lineRenderer.gameObject.GetComponent<RectTransform>();

            // Convert to UI space and draw - avoid try/catch here because C# forbids yield inside a try block
            var uiA = ToUISpace(rectA);
            var uiB = ToUISpace(rectB);

            _lineRenderer.AddPoints(new[] { uiA, uiB });
            Canvas.ForceUpdateCanvases();
            Debug.Log($"[HintManager] Start: recreated connection '{key}' with ui points {uiA} - {uiB}, color={_lineRenderer.color}");
        }
    }

    private void Start()
    {
        Init();

        if (_unlockedHints.Count == 0)
        {
            DialogCanvas.Spawn(startDialog, () =>
            {
                foreach (var startHint in startHints)
                {
                    UnlockHint(startHint);
                }
            });
        }
    }

    public bool IsHintStepCompleted(Hint hint, HintStep step)
    {
        var failed = step.NeededConnectedHints.Select(stepNeededConnectedHint => BuildHintKey(hint, stepNeededConnectedHint)).Any(key => !_hintConnectionsKeys.Contains(key));
        if (!failed) return true;
        failed = step.AlternativeConnectedHints.Select(stepNeededConnectedHint => BuildHintKey(hint, stepNeededConnectedHint)).Any(key => !_hintConnectionsKeys.Contains(key));
        return !failed;
    }

    public void UnlockHint(Hint hint)
    {
        if (_unlockedHints.Contains(hint.name))
        {
            return;
        }
        _unlockedHints.Add(hint.name);
        var connection = FindConnectorByHintName(hint.name);
        connection.gameObject.SetActive(true);
        PlayerPrefs.SetString("hint_unlocked", string.Join(",", _unlockedHints));
        PlayerPrefs.Save();
    }
    
    public void ShowHint(Hint hint)
    {
        CurrentHint = hint;
        var key = "hints_" + hint.name;
        var currentHintStep = PlayerPrefs.GetInt(key, 0);
        CurrentHintStep = hint.hints[currentHintStep];

        if (hint.hints == null || hint.hints.Length == 0)
        {
            Debug.LogWarning($"[HintManager] ShowHint: hint '{hint.name}' has no hint steps.");
            if (_title != null) _title.text = hint.GetTitle + " (0/0)";
            if (_description != null) _description.text = "";
            return;
        }

        if (currentHintStep < 0 || currentHintStep >= hint.hints.Length)
        {
            Debug.LogWarning($"[HintManager] ShowHint: stored step {currentHintStep} out of range for hint '{hint.name}'. Clamping.");
            currentHintStep = Mathf.Clamp(currentHintStep, 0, hint.hints.Length - 1);
        }

        if (_title != null)
            _title.text = hint.GetTitle + "(" + currentHintStep + "/" + hint.hints.Length + ")";
        else
            Debug.LogWarning("[HintManager] ShowHint: _title TMP_Text is not assigned");

        if (_description != null)
            _description.text = CurrentHintStep.GetDescription ?? string.Empty;
        else
            Debug.LogWarning("[HintManager] ShowHint: _description TMP_Text is not assigned");
        
        ShowHintSubBar();

        Debug.Log($"[HintManager] ShowHint: Showing hint '{hint.name}' step {currentHintStep}/{hint.hints.Length}. Title='{hint.GetTitle}'");
    }
    
    private void ShowHintSubBar()
    {
        // Hint image
        _image.gameObject.SetActive(CurrentHintStep.HintImage != null);
        if(_image.gameObject.activeSelf) _image.sprite = CurrentHintStep.HintImage;
    }

    public bool AddHintConnection(Hint hintOne, Hint hintTwo)
    {
        var key = BuildHintKey(hintOne, hintTwo);

        // Already exists?
        if (_hintConnectionsKeys.Any(existing => existing == key))
        {
            Debug.Log("[HintManager] AddHintConnection: connection already exists, skipping.");
            return false;
        }
        
        try
        {
            var connectorOne = FindConnectorByHintName(hintOne.name);
            var connectorTwo = FindConnectorByHintName(hintTwo.name);

            var rectOne = connectorOne.GetComponent<RectTransform>();
            var rectTwo = connectorTwo.GetComponent<RectTransform>();

            Debug.Log($"[HintManager] AddHintConnection: connector world positions - one={rectOne.position}, two={rectTwo.position}");

            var uiPosOne = ToUISpace(rectOne);
            var uiPosTwo = ToUISpace(rectTwo);

            _lineRenderer.AddPoints(new []
            {
                uiPosOne,
                uiPosTwo
            });
            Debug.Log($"[HintManager] AddHintConnection: added points, color={_lineRenderer.color}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[HintManager] AddHintConnection: failed to create connection: {ex}");
            return false;
        }
        
        _hintConnectionsKeys.Add(key);
        
        var joined = string.Join(",", _hintConnectionsKeys);
        PlayerPrefs.SetString("hint_connections", joined);
        PlayerPrefs.Save();

        Debug.Log($"[HintManager] AddHintConnection: connection '{key}' added and saved. Total connections: {_hintConnectionsKeys.Count}");
        return true;
    }

    private string BuildHintKey(Hint hint1, Hint hint2)
    {
        var hintNames = new[] { hint1.name, hint2.name };
        Array.Sort(hintNames, StringComparer.Ordinal);
        var key = string.Join("_", hintNames);
        return key;
    }

    private Vector2 ToUISpace(RectTransform sourceRect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform,
            sourceRect.position,
            null,
            out var localPoint
        );
        
        return localPoint;
    }
    
    private UIDragConnector FindConnectorByHintName(string hintName)
    {
        if (_connectors == null || _connectors.Length == 0)
            return null;

        return _connectors.Where(uiDragConnector => uiDragConnector is not null)
            .FirstOrDefault(uiDragConnector => uiDragConnector.Hint is not null && uiDragConnector.Hint.name == hintName);
    }
}
