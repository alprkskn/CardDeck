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
    }

    public void ReorderCards()
    {
        var step = 1f / _cards.Count;

        step = Mathf.Min(step, _maxCardOffset);

        var totalLength = step * (_cards.Count - 1);

        var start = (1f - totalLength) * 0.5f;

        for(int i = 0; i < _cards.Count; i++)
        {
            Vector3 normal;
            Vector3 tangent;
            Vector3 position = _deckBezier.GetPoint(start + step * i, out tangent, out normal);

            var angle = Vector3.SignedAngle(Vector3.up, normal, Vector3.forward);

            _cards[i].SetTarget(position, Quaternion.AngleAxis(angle, Vector3.forward));
            _cards[i].RectTransform.SetSiblingIndex(i);
        }
    }
}
