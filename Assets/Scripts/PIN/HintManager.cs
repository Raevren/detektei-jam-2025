using System;
using System.Collections.Generic;
using System.Linq;
using PIN;
using TMPro;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _title;

    [SerializeField]
    private TMP_Text _description;

    [SerializeField]
    private UILineRenderer _lineRenderer;
    
    [SerializeField]
    private UIDragConnector[] _connectors;
    
    private RectTransform _rectTransform;
    
    private List<string> _hintConnectionsKeys = new();

    private void Init()
    {
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
    }

    public void ShowHint(Hint hint)
    {
        var key = "hints_" + hint.name;
        var currentHintStep = PlayerPrefs.GetInt(key, 0);

        if (hint.hints == null || hint.hints.Length == 0)
        {
            Debug.LogWarning($"[HintManager] ShowHint: hint '{hint.name}' has no hint steps.");
            if (_title != null) _title.text = hint.hintTitle + " (0/0)";
            if (_description != null) _description.text = "";
            return;
        }

        if (currentHintStep < 0 || currentHintStep >= hint.hints.Length)
        {
            Debug.LogWarning($"[HintManager] ShowHint: stored step {currentHintStep} out of range for hint '{hint.name}'. Clamping.");
            currentHintStep = Mathf.Clamp(currentHintStep, 0, hint.hints.Length - 1);
        }

        if (_title != null)
            _title.text = hint.hintTitle + "(" + currentHintStep + "/" + hint.hints.Length + ")";
        else
            Debug.LogWarning("[HintManager] ShowHint: _title TMP_Text is not assigned");

        if (_description != null)
            _description.text = hint.hints[currentHintStep].description ?? string.Empty;
        else
            Debug.LogWarning("[HintManager] ShowHint: _description TMP_Text is not assigned");

        Debug.Log($"[HintManager] ShowHint: Showing hint '{hint.name}' step {currentHintStep}/{hint.hints.Length}. Title='{hint.hintTitle}'");
    }

    public bool AddHintConnection(Hint hintOne, Hint hintTwo)
    {
        // Build a sorted pair key from the two hint names
        var hintNames = new[] { hintOne.name, hintTwo.name };
        Array.Sort(hintNames, StringComparer.Ordinal);
        var key = string.Join("_", hintNames);

        Debug.Log($"[HintManager] AddHintConnection: Attempting to add connection between '{hintNames[0]}' and '{hintNames[1]}', key='{key}'");

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
