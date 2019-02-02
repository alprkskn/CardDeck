using System;
using System.Collections.Generic;
using UnityEngine;

public static class CardUtils
{
    public static readonly int KindCount = Enum.GetNames(typeof(CardKinds)).Length;
    public static readonly int ValueCount = Enum.GetNames(typeof(CardValues)).Length;
    public static readonly int CardCount = KindCount * ValueCount;

    public const int MinAcceptedStreak = 3;

    private static readonly CardStraightComparer _straightComparer = new CardStraightComparer();
    private static readonly CardMatchingComparer _matchingComparer = new CardMatchingComparer();

    public static CardKinds GetKindById(int id)
    {
        return (CardKinds)(id / ValueCount);
    }

    public static CardValues GetValueById(int id)
    {
        return (CardValues)(id % ValueCount);
    }

    public static int GetId(this CardInfo info)
    {
        return (int)info.Kind * ValueCount + (int)info.Value;
    }

    public static void StraightSort(List<CardInfo> deck)
    {
        // First sort the deck by kinds and values.
        deck.Sort(_straightComparer);

        // Iterate from the end to start. Discard mismatching cards and append to the end of the list.
        int cursor = deck.Count - 1;

        int currentStreak = 0;

        while(cursor > 0)
        {
            // Compare the card with the previous one
            if(deck[cursor].Kind != deck[cursor - 1].Kind)
            {
                // If we skipped kinds, check if you have a streak. If not append the current card to the end.
                EndStreak(deck, cursor, ref currentStreak);
            }
            else
            {
                // Check the value difference. If its not 1, again check the current streak
                var diff = Mathf.Abs((int)deck[cursor].Value - (int)deck[cursor - 1].Value);

                if(diff > 1)
                {
                    EndStreak(deck, cursor, ref currentStreak);
                }
                else
                {
                    currentStreak++;
                }
            }

            cursor--;
        }

        EndStreak(deck, 0, ref currentStreak);
    }

    public static void MatchingSort(List<CardInfo> deck)
    {
        // First sort the deck by kinds and values.
        deck.Sort(_matchingComparer);

        // Iterate from the end to start. Discard mismatching cards and append to the end of the list.
        int cursor = deck.Count - 1;

        int currentStreak = 0;

        while (cursor > 0)
        {
            if (deck[cursor].Value != deck[cursor - 1].Value)
            {
                EndStreak(deck, cursor, ref currentStreak);
            }
            else
            {
                currentStreak++;
            }

            cursor--;
        }

        EndStreak(deck, 0, ref currentStreak);
    }

    private static void EndStreak(List<CardInfo> deck, int cursor, ref int streak)
    {
        if(streak < MinAcceptedStreak - 1)
        {
            for(int i = streak; i >= 0; i--)
            {
                var info = deck[cursor + i];
                deck.RemoveAt(cursor + i);
                deck.Add(info);
            }
        }
        else
        {
            // Leave the current streak where they are. Just end the streak
            // TODO: Here, we can collect the info of the current streak if necessary.
            Debug.Log("Found streak of " + (streak + 1));
        }

        streak = 0;
    }
}

public class CardStraightComparer : IComparer<CardInfo>
{
    public int Compare(CardInfo x, CardInfo y)
    {
        return x.GetId().CompareTo(y.GetId());
    }
}

public class CardMatchingComparer : IComparer<CardInfo>
{
    public int Compare(CardInfo x, CardInfo y)
    {
        var valueComparison = x.Value.CompareTo(y.Value);

        if(valueComparison == 0)
        {
            return x.Kind.CompareTo(y.Kind);
        }

        return valueComparison;
    }
}
