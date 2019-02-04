using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private DeckBehaviour _playerDeck;

    [SerializeField] private Sprite[] SymbolImages;

    public Sprite GetSymbolImage(CardKinds kind)
    {
        return SymbolImages[(int)kind];
    }

    // Use this for initialization
    void Start()
    {
        _playerDeck.Initialize();

        var deck = CardUtils.GetUniqueDeck(11);

        for(int i = 0; i < deck.Length; i++)
        {
            var go = Instantiate(_cardPrefab);
            var card = go.GetComponent<CardBehaviour>();
            card.SetCardInfo(deck[i]);
            _playerDeck.AddCard(card);
        }
    }
}
