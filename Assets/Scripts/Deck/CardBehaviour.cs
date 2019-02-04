using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    private const float PositionDamping = 0.8f;
    private const float RotationDamping = 0.8f;
    private const float HighlightCoroutineDuration = 0.1f;
    private const float HoverHighlightDuration = 0.3f;

    public event Action<CardBehaviour> CursorEnter;
    public event Action<CardBehaviour> CursorExit;
    public event Action<CardBehaviour> CursorDown;
    public event Action<CardBehaviour> CursorUp;

    [SerializeField] private Image _highlight;
    [SerializeField] private Image _subImage;
    [SerializeField] private Text _text;

    private Vector3? _targetPosition;
    private Quaternion? _targetRotation;
    private RectTransform _rectTransform;

    [SerializeField] private CardInfo _info;
    private Coroutine _highlightCoroutine;
    private Coroutine _cursorHoverCoroutine;

    public CardInfo Info
    {
        get { return _info; }
    }

    public RectTransform RectTransform
    {
        get
        {
            if (!_rectTransform)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    private void Update()
    {
        if (_targetRotation.HasValue)
        {
            RectTransform.rotation = Quaternion.Lerp(RectTransform.rotation, _targetRotation.Value, RotationDamping);
        }

        if (_targetPosition.HasValue)
        {
            RectTransform.position = Vector3.Lerp(RectTransform.position, _targetPosition.Value, PositionDamping);
        }
    }

    public void SetTarget(Vector3 position, Quaternion rotation, bool immediate = false)
    {
        _targetPosition = position;
        _targetRotation = rotation;

        if (immediate)
        {
            RectTransform.position = position;
            RectTransform.rotation = rotation;
        }
    }

    public void ToggleHighlight(bool on)
    {
        if(_highlightCoroutine != null)
        {
            StopCoroutine(_highlightCoroutine);
        }

        _highlightCoroutine = StartCoroutine(HighlightCoroutine(on));
    }

    public void SetCardInfo(CardInfo info)
    {
        _info = info;

        _text.text = info.ValueText;
        _text.color = info.ValueColor;
        _subImage.sprite = BoardManager.Instance.GetSymbolImage(info.Kind);
    }

    #region EventSystem Listeners
    public void OnPointerEnter(BaseEventData eventArgs)
    {
        _highlightCoroutine = StartCoroutine(HoverCoroutine());
    }

    public void OnPointerExit(BaseEventData eventArgs)
    {
        if(_highlightCoroutine != null)
        {
            StopCoroutine(_highlightCoroutine);
            _highlightCoroutine = null;
        }

        if (CursorExit != null) CursorExit(this);
    }

    public void OnPointerDown(BaseEventData eventArgs)
    {
        if (CursorDown != null) CursorDown(this);
    }

    public void OnPointerUp(BaseEventData eventArgs)
    {
        if (CursorUp != null) CursorUp(this);
    }
    #endregion

    private IEnumerator HighlightCoroutine(bool on)
    {
        var initialAlpha = _highlight.color.a;

        float timer = 0f;

        _highlight.enabled = true;

        while(timer < HighlightCoroutineDuration)
        {
            _highlight.SetAlpha(Mathf.Lerp(initialAlpha, on ? 1f : 0f, timer / HighlightCoroutineDuration));
            yield return null;
            timer += Time.deltaTime;
        }

        _highlight.SetAlpha(on ? 1f : 0f);
        _highlight.enabled = on;
        _highlightCoroutine = null;
    }

    private IEnumerator HoverCoroutine()
    {
        yield return new WaitForSeconds(HoverHighlightDuration);
        _highlightCoroutine = null;
        if (CursorEnter != null) CursorEnter(this);
    }
}
