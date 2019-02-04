using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private DeckBehaviour _playerDeck;

    [SerializeField] private Sprite[] SymbolImages;
    [SerializeField] private Button _shuffleButton;

    private bool _shuffling;

    public Sprite GetSymbolImage(CardKinds kind)
    {
        return SymbolImages[(int)kind];
    }

    // Use this for initialization
    void Start()
    {
        _playerDeck.Initialize();
    }

    public void Shuffle()
    {
        if (!_shuffling)
        {
            _playerDeck.ClearDeck();

            var deck = CardUtils.GetUniqueDeck(11);

            StartCoroutine(ShuffleCoroutine(deck, 0.075f));
        }
    }

    public void DealTestCase()
    {
        if(!_shuffling)
        {
            _playerDeck.ClearDeck();
            var deck = CardUtils.GetTestCaseDeck();
            StartCoroutine(ShuffleCoroutine(deck, 0.075f));
        }
    }

    private IEnumerator ShuffleCoroutine(CardInfo[] deck, float nextCardDuration)
    {
        _shuffling = true;
        _shuffleButton.interactable = false;

        for(int i = 0; i < deck.Length; i++)
        {
            var go = Instantiate(_cardPrefab);
            var card = go.GetComponent<CardBehaviour>();
            card.SetCardInfo(deck[i]);
            _playerDeck.AddCard(card);

            yield return new WaitForSeconds(nextCardDuration);
        }

        _shuffling = false;
        _shuffleButton.interactable = true;
    }
}
