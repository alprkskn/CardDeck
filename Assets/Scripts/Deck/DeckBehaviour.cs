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

    private const float CardHighlightOffset = 45f;

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
        for(int i = 0; i < _cards.Count; i++)
        {
            Vector3 normal;
            Vector3 position = _deckBezier.GetPoint(GetCardPositionNormalized(i), out normal);

            SetCardTarget(_cards[i], position, normal);
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

    #region Card Events
    private void Card_CursorUp(CardBehaviour sender)
    {

    }

    private void Card_CursorDown(CardBehaviour sender)
    {
    }

    private void Card_CursorExit(CardBehaviour sender)
    {
        Vector3 normal;

        var index = _cards.FindIndex((x) => x == sender);
        var pos = _deckBezier.GetPoint(GetCardPositionNormalized(index), out normal);
        SetCardTarget(sender, pos, normal);

        sender.ToggleHighlight(false);
        sender.RectTransform.SetSiblingIndex(index);
        _highlighted = null;
    }

    private void Card_CursorEnter(CardBehaviour sender)
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

    private void SetCardTarget(CardBehaviour card, Vector3 pos, Vector3 normal)
    {
        var angle = Vector3.SignedAngle(Vector3.up, normal, Vector3.forward);

        card.SetTarget(pos, Quaternion.AngleAxis(angle, Vector3.forward));
    }

    #endregion
}
