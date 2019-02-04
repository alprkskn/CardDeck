using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private DeckBehaviour _playerDeck;

    // Use this for initialization
    void Start()
    {
        _playerDeck.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var go = Instantiate(_cardPrefab);
            var card = go.GetComponent<CardBehaviour>();
            _playerDeck.AddCard(card);
        }
    }
}
