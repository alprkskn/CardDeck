using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] _bezierPoints;
    [SerializeField] private Collider2D _deckBounds;

    [SerializeField] private float _maxCardOffset;

    [SerializeField] private RectTransform _cardsParent;

    private BezierSpline _deckBezier;
    private List<CardBehaviour> _cards;

    private CardBehaviour _highlighted;
    private CardBehaviour _selected;
    private int _lastSelectionIndex;

    private Coroutine _moveCardCoroutine;

    private const float CardHighlightOffset = 45f;
    private const float CardInsertionThreshold = 45f;

    public void Initialize()
    {
        _deckBezier = new BezierSpline(_bezierPoints);
        _cards = new List<CardBehaviour>();
    }

    public void AddCard(CardBehaviour card)
    {
        _cards.Add(card);
        card.RectTransform.SetParent(_cardsParent, true);
        ReorderCards();

        RegisterCardEvents(card);
    }

    public void ReorderCards()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            Vector3 normal;
            Vector3 position = _deckBezier.GetPoint(GetCardPositionNormalized(i), out normal);

            if (_cards[i] != _selected)
            {
                SetCardTarget(_cards[i], position, normal);
            }

            _cards[i].RectTransform.SetSiblingIndex(i);
        }
    }

    private void RegisterCardEvents(CardBehaviour card)
    {
        card.CursorEnter += Card_CursorEnter;
        card.CursorExit += Card_CursorExit;
        card.CursorDown += Card_CursorDown;
        card.CursorUp += Card_CursorUp;
    }

    private void UnregisterCardEvents(CardBehaviour card)
    {
        card.CursorEnter -= Card_CursorEnter;
        card.CursorExit -= Card_CursorExit;
        card.CursorDown -= Card_CursorDown;
        card.CursorUp -= Card_CursorUp;
    }

    private float GetCardPositionNormalized(int i)
    {
        var step = 1f / _cards.Count;

        step = Mathf.Min(step, _maxCardOffset);

        var totalLength = step * (_cards.Count - 1);
        var start = (1f - totalLength) * 0.5f;

        return start + step * i;
    }

    private int GetCardIndexFromT(float t)
    {
        var step = 1f / _cards.Count;

        step = Mathf.Min(step, _maxCardOffset);

        var totalLength = step * (_cards.Count - 1);
        var start = (1f - totalLength) * 0.5f;

        float cursor = start;

        int selectedIndex = 0;
        float selectedT = start;


        while (t > cursor && t <= 1f)
        {
            selectedIndex++;
            cursor += step;
            selectedT = cursor;
        }

        if (selectedIndex < _cards.Count - 1)
        {
            var diffPrev = t - selectedT;
            var diffNext = (selectedT + step) - t;

            if (diffNext < diffPrev)
            {
                selectedIndex++;
                selectedT = selectedT + step;
            }
        }

        var i = Mathf.Clamp(selectedIndex, 0, _cards.Count - 1);
        return i;
    }

    private int GetCardIndexWithPosition(Vector3 position)
    {
        float t;

        var closestPoint = _deckBezier.FindClosestPoint(position, out t);

        if (Vector2.Distance(position, closestPoint) < CardInsertionThreshold)
        {
            var index = GetCardIndexFromT(t);
            return index;
        }
        else
        {
            return -1;
        }

    }

    #region Card Events
    private void Card_CursorUp(CardBehaviour sender)
    {
        _cards.Remove(sender);

        var index = GetCardIndexWithPosition(sender.RectTransform.position);
        if (index > 0)
        {
            _cards.Insert(index, sender);
        }
        else
        {
            // If we find no where to put card, put it back to the original place
            _cards.Insert(_lastSelectionIndex, sender);
        }

        _selected = null;
        StopCoroutine(_moveCardCoroutine);
        _moveCardCoroutine = null;
        ReorderCards();
    }

    private void Card_CursorDown(CardBehaviour sender)
    {
        sender.ToggleHighlight(false);
        _highlighted = null;
        _selected = sender;

        _moveCardCoroutine = StartCoroutine(MoveCard(sender));
    }

    private void Card_CursorExit(CardBehaviour sender)
    {
        if (!_selected)
        {
            Vector3 normal;

            var index = _cards.FindIndex((x) => x == sender);
            var pos = _deckBezier.GetPoint(GetCardPositionNormalized(index), out normal);
            SetCardTarget(sender, pos, normal);

            sender.ToggleHighlight(false);
            sender.RectTransform.SetSiblingIndex(index);
            _highlighted = null;
        }
    }

    private void Card_CursorEnter(CardBehaviour sender)
    {
        if (!_selected)
        {
            Vector3 normal;

            var pos = _deckBezier.GetPoint(GetCardPositionNormalized(_cards.FindIndex((x) => x == sender)), out normal);

            if (_highlighted)
            {
                Card_CursorExit(_highlighted);
            }

            sender.SetTarget(pos + normal * CardHighlightOffset, Quaternion.identity);
            sender.ToggleHighlight(true);
            sender.RectTransform.SetAsLastSibling();
            _highlighted = sender;
        }
    }

    private void SetCardTarget(CardBehaviour card, Vector3 pos, Vector3 normal)
    {
        var angle = Vector3.SignedAngle(Vector3.up, normal, Vector3.forward);

        card.SetTarget(pos, Quaternion.AngleAxis(angle, Vector3.forward));
    }

    private IEnumerator MoveCard(CardBehaviour card)
    {
        _lastSelectionIndex = _cards.FindIndex((x) => x == card);

        int prevIndex = _lastSelectionIndex;

        while (true)
        {
            var cursorPosition = Input.mousePosition;

            if (Input.touchCount > 0)
            {
                cursorPosition = Input.GetTouch(0).position;
            }

            card.SetTarget(cursorPosition, Quaternion.identity);

            var index = GetCardIndexWithPosition(cursorPosition);
            if (index >= 0 && index != prevIndex)
            {
                _cards.Remove(card);
                _cards.Insert(index, card);
                ReorderCards();
                card.RectTransform.SetSiblingIndex(_cards.Count);
                prevIndex = index;
            }

            yield return null;
        }
    }
    #endregion
}