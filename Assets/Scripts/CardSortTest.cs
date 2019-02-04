using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CardSortTest
{
    private List<CardInfo> _deck;
    private SortMethod _method;

    public CardSortTest(int deckCount, SortMethod method)
    {
        _deck = new List<CardInfo>(deckCount);
        _method = method;
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

    public CardSortTest(int[] deck, SortMethod method)
    {
        _deck = new List<CardInfo>(deck.Length);
        _method = method;

        for(int i = 0; i < deck.Length; i++)
        {
            _deck.Add(CardInfo.FromId(deck[i]));
        }
    }

    public void ExecuteAndLog()
    {
        Debug.Log("Initial deck:\n" + ListCards());
        switch (_method)
        {
            case SortMethod.Straight:
                CardUtils.StraightSort(_deck);
                break;
            case SortMethod.Matching:
                CardUtils.MatchingSort(_deck);
                break;
            case SortMethod.Smart:
                CardUtils.SmartSort(_deck);
                break;
        }
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
