using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CardSortTest
{
    private List<CardInfo> _deck;

    public CardSortTest(int deckCount)
    {
        _deck = new List<CardInfo>(deckCount);
        HashSet<int> cardSet = new HashSet<int>();

        while(cardSet.Count < deckCount)
        {
            cardSet.Add(Random.Range(0, CardUtils.CardCount));
        }

        foreach(var id in cardSet)
        {
            _deck.Add(CardInfo.FromId(id));
        }
    }

    public CardSortTest(int[] deck)
    {
        _deck = new List<CardInfo>(deck.Length);

        for(int i = 0; i < deck.Length; i++)
        {
            _deck.Add(CardInfo.FromId(deck[i]));
        }
    }

    public void ExecuteAndLog()
    {
        Debug.Log("Initial deck:\n" + ListCards());
        CardUtils.StraightSort(_deck);
        Debug.Log("Sorted deck:\n" + ListCards());
    }

    private string ListCards()
    {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < _deck.Count; i++)
        {
            sb.AppendLine(_deck[i].ToString());
        }

        return sb.ToString();
    }
}
