
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace PIN
{
    public class UIDragConnector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Hint _hint;
        
        [SerializeField]
        private Sprite _alternativeSprite;
        [SerializeField]
        private Sprite _alternativeSpriteTwo;
        
        public Hint Hint => _hint;

        private RectTransform _rectTransform;
        private UILineRenderer _lineRenderer;
        private HintManager _hintManager;

        // neue Felder zum Unterscheiden von Klick vs Drag
        private Vector2 _pressScreenPos;
        private bool _isDragging;
        private int _pixelDragThreshold = -1; // lazily init
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _hintManager = GetComponentInParent<HintManager>();
            
            if(_hintManager.HasUncompletedHintSteps(_hint))
            {
                StartCoroutine(Animate());
            }
        }

        public IEnumerator ShowDialogAvailable()
        {
            while (_hintManager is null)
            {
                yield return new WaitForFixedUpdate();
            }
            
            var image = gameObject.GetComponent<Image>();
            var originalSprite = image.sprite;
            
            while (_hintManager.HintStepHasNewDialog(_hint))
            {
                // Wechsel zu alternativer Sprite
                image.sprite = _alternativeSpriteTwo;
                yield return new WaitForSeconds(0.5f);

                // Zurück zur Original-Sprite
                image.sprite = originalSprite;
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator Animate()
        {
            var image = gameObject.GetComponent<Image>();
            var originalSprite = image.sprite;
            
            while (_hintManager.HasUncompletedHintSteps(_hint))
            {
                // Wechsel zu alternativer Sprite
                image.sprite = _alternativeSprite;
                yield return new WaitForSeconds(0.5f);

                // Zurück zur Original-Sprite
                image.sprite = originalSprite;
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Merken, wo der Pointer gedrückt wurde
        public void OnPointerDown(PointerEventData eventData)
        {
            _pressScreenPos = eventData.position;
            _isDragging = false;
        }

        // Wenn das EventSystem entscheidet, dass ein Drag startet
        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;

            var lineRendererGo = new GameObject("UIEmpty", typeof(RectTransform), typeof(CanvasRenderer));
            lineRendererGo.transform.SetParent(transform, false);

            var rt = lineRendererGo.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            _lineRenderer = lineRendererGo.AddComponent<UILineRenderer>();
            _lineRenderer.color = Color.black;

            _lineRenderer.SetPoints(new List<Vector2[]>());
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_lineRenderer == null)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.pressPosition, eventData.pressEventCamera, out var startPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out var currentPos);

            _lineRenderer.SetPoints(new List<Vector2[]> { new[] { startPos, currentPos } });
        }

        // Wenn der Pointer losgelassen wurde, aber kein Drag gestartet wurde -> Klick
        public void OnPointerUp(PointerEventData eventData)
        {
            // Falls bereits dragging, dann wird OnEndDrag später aufgerufen
            if (_isDragging)
                return;

            // Lazy init Threshold
            if (_pixelDragThreshold < 0)
                _pixelDragThreshold = EventSystem.current != null ? EventSystem.current.pixelDragThreshold : 5;

            var distance = Vector2.Distance(_pressScreenPos, eventData.position);

            if (distance <= _pixelDragThreshold)
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            SoundSystem.Instance.PlayGenericSfx(CommonSfx.ClickPin);
            _hintManager.ShowHint(_hint);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            var connectedTo = (from result in results where result.gameObject != gameObject select result.gameObject.GetComponent<UIDragConnector>() into connectedConnector where connectedConnector != null select connectedConnector._hint).FirstOrDefault();

            if (connectedTo != null)
            {
                _hintManager.AddHintConnection(_hint, connectedTo); 
            }

            if (_lineRenderer == null) return;
            Destroy(_lineRenderer.gameObject);
            _lineRenderer = null;
        }
    }
}
