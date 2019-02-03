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

    #region Regular Sorting
    public static List<CardInfo[]> StraightSort(List<CardInfo> deck)
    {
        // First sort the deck by kinds and values.
        deck.Sort(_straightComparer);

        List<CardInfo[]> groups = new List<CardInfo[]>();

        CardInfo[] temp;

        // Iterate from the end to start. Discard mismatching cards and append to the end of the list.
        int cursor = deck.Count - 1;

        int currentStreak = 0;

        while (cursor > 0)
        {
            // Compare the card with the previous one
            if (deck[cursor].Kind != deck[cursor - 1].Kind)
            {
                // If we skipped kinds, check if you have a streak. If not append the current card to the end.
                temp = EndStreak(deck, cursor, ref currentStreak);

                if (temp != null)
                {
                    groups.Add(temp);
                }
            }
            else
            {
                // Check the value difference. If its not 1, again check the current streak
                var diff = Mathf.Abs((int)deck[cursor].Value - (int)deck[cursor - 1].Value);

                if (diff > 1)
                {
                    temp = EndStreak(deck, cursor, ref currentStreak);

                    if (temp != null)
                    {
                        groups.Add(temp);
                    }
                }
                else
                {
                    currentStreak++;
                }
            }

            cursor--;
        }

        temp = EndStreak(deck, 0, ref currentStreak);

        if (temp != null)
        {
            groups.Add(temp);
        }

        return groups;
    }

    public static List<CardInfo[]> MatchingSort(List<CardInfo> deck)
    {
        // First sort the deck by kinds and values.
        deck.Sort(_matchingComparer);

        List<CardInfo[]> groups = new List<CardInfo[]>();

        CardInfo[] temp;
        // Iterate from the end to start. Discard mismatching cards and append to the end of the list.
        int cursor = deck.Count - 1;

        int currentStreak = 0;

        while (cursor > 0)
        {
            if (deck[cursor].Value != deck[cursor - 1].Value)
            {
                temp = EndStreak(deck, cursor, ref currentStreak);

                if (temp != null)
                {
                    groups.Add(temp);
                }
            }
            else
            {
                currentStreak++;
            }

            cursor--;
        }

        temp = EndStreak(deck, 0, ref currentStreak);

        if (temp != null)
        {
            groups.Add(temp);
        }

        return groups;
    }

    private static CardInfo[] EndStreak(List<CardInfo> deck, int cursor, ref int streak)
    {
        CardInfo[] cardGroup = null;

        if (streak < MinAcceptedStreak - 1)
        {
            for (int i = streak; i >= 0; i--)
            {
                var info = deck[cursor + i];
                deck.RemoveAt(cursor + i);
                deck.Add(info);
            }
        }
        else
        {
            // Leave the current streak where they are. Just end the streak
            // Here, we can collect the info of the current streak.
            cardGroup = new CardInfo[streak + 1];
            for (int i = 0; i < streak + 1; i++)
            {
                cardGroup[i] = deck[cursor + i];
            }

            Debug.Log("Found streak of " + (streak + 1));
        }

        streak = 0;

        return cardGroup;
    }
    #endregion

    #region Smart Sorting
    public static List<CardInfo[]> SmartSort(List<CardInfo> deck)
    {
        // Collect straight and matching groups first
        var straightGroups = StraightSort(deck);
        var matchingGroups = MatchingSort(deck);

        // Populate group lists with all possible combinations.
        PopulateStraights(straightGroups);
        PopulateMatchings(matchingGroups);

        var possibleCardGroups = new List<CardInfo[]>(straightGroups.Count + matchingGroups.Count);
        possibleCardGroups.AddRange(straightGroups);
        possibleCardGroups.AddRange(matchingGroups);

        var maxNumberOfgroups = deck.Count / MinAcceptedStreak;

        List<List<CardInfo[]>> possibleGroupCombinations = new List<List<CardInfo[]>>();

        for(int i = 1; i <= maxNumberOfgroups; i++)
        {
            var tempCombination = new List<CardInfo[]>(i);
            for(int j = 0; j < i; j++)
            {
                tempCombination.Add(new CardInfo[0]);
            }

            CombinationHelper<CardInfo[]>(possibleGroupCombinations, possibleCardGroups, tempCombination, 0, possibleCardGroups.Count - 1, 0, i);
        }

        int maxScore = int.MinValue;
        List<CardInfo[]> selected = null;
        HashSet<CardInfo> usedCards = null;
        foreach(var grouping in possibleGroupCombinations)
        {
            var score = 0;
            HashSet<CardInfo> tempCards;

            if(CheckGrouping(grouping, out tempCards, out score))
            {
                if(score > maxScore)
                {
                    maxScore = score;
                    selected = grouping;
                    usedCards = tempCards;
                }
            }
        }

        for(int i = deck.Count - 1; i >= 0; i--)
        {
            if (usedCards.Contains(deck[i]))
            {
                deck.RemoveAt(i);
            }
        }

        int index = 0;
        foreach(var group in selected)
        {
            foreach(var card in group)
            {
                deck.Insert(index, card);
                index++;
            }
        }

        return selected;
    }
    #region Extend Groups
    private static void PopulateStraights(List<CardInfo[]> groups)
    {
        var initialCount = groups.Count;

        for (int i = 0; i < initialCount; i++)
        {
            var group = groups[i];

            // If there are possible smaller accepted groups
            // Add those to the list.
            if (group.Length > MinAcceptedStreak)
            {
                for (int length = MinAcceptedStreak; length < group.Length; length++)
                {
                    groups.AddRange(GetSubSequences(group, length));
                }
            }
        }
    }

    private static void PopulateMatchings(List<CardInfo[]> groups)
    {
        var initialCount = groups.Count;

        for (int i = 0; i < initialCount; i++)
        {
            var group = groups[i];

            if(group.Length > MinAcceptedStreak)
            {
                for(int length = MinAcceptedStreak; length < group.Length; length++)
                {
                    groups.AddRange(GetCombinations(group, length));
                }
            }
        }

    }

    private static List<CardInfo[]> GetSubSequences(CardInfo[] group, int length)
    {
        var result = new List<CardInfo[]>();

        for (int i = 0; i <= group.Length - length; i++)
        {
            var subSequence = new CardInfo[length];

            for (int j = 0; j < length; j++)
            {
                subSequence[j] = group[i + j];
            }

            result.Add(subSequence);
        }

        return result;
    }

    private static List<CardInfo[]> GetCombinations(CardInfo[] group, int length)
    {
        var result = new List<CardInfo[]>();

        var temp = new CardInfo[length];
        CombinationHelper<CardInfo>(result, group, temp, 0, group.Length - 1, 0, length);

        return result;
    }

    private static void CombinationHelper<T>(List<List<T>> store, List<T> input, List<T> combination
                                            , int startIndex, int endIndex
                                            , int curIndex, int size)
    {
        // Terminate when index reaches the desired length
        if(curIndex == size)
        {
            // copy the combination array and store it.
            var temp = new List<T>(combination.Count);
            temp.AddRange(combination);
            store.Add(temp);
            return;
        }

        for(int i = startIndex; i <= endIndex
            && endIndex - i + 1 >= size - curIndex; i++)
        {
            combination[curIndex] = input[i];
            CombinationHelper(store, input, combination, i + 1, endIndex, curIndex + 1, size);
        }
    }

    private static void CombinationHelper<T>(List<T[]> store, T[] input, T[] combination
                                            , int startIndex, int endIndex
                                            , int curIndex, int size)
    {
        // Terminate when index reaches the desired length
        if(curIndex == size)
        {
            // copy the combination array and store it.
            var temp = new T[combination.Length];
            combination.CopyTo(temp, 0);
            store.Add(temp);
            return;
        }

        for(int i = startIndex; i <= endIndex
            && endIndex - i + 1 >= size - curIndex; i++)
        {
            combination[curIndex] = input[i];
            CombinationHelper(store, input, combination, i + 1, endIndex, curIndex + 1, size);
        }
    }
    #endregion

    private static bool CheckGrouping(List<CardInfo[]> groups, out HashSet<CardInfo> usedCards, out int score)
    {
        usedCards = new HashSet<CardInfo>();

        var totalCount = 0;
        score = 0;

        foreach (var group in groups)
        {
            totalCount += group.Length;
            foreach(var card in group)
            {
                if (usedCards.Add(card))
                {
                    score += card.CardScore;
                }
                else
                {
                    score = -1;
                    usedCards.Clear();
                    return false;
                }
            }
        }

        return true;
    }

    private static bool CheckOverlaps(CardInfo[] first, CardInfo[] second)
    {
        for(int i = 0; i < first.Length; i++)
        {
            for(int j = 0; j < second.Length; j++)
            {
                if (first[i].Equals(second[j]))
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion
}

#region Comparers
public class CardStraightComparer : IComparer<CardInfo>
{
    public int Compare(CardInfo x, CardInfo y)
    {
        return x.Id.CompareTo(y.Id);
    }
}

public class CardMatchingComparer : IComparer<CardInfo>
{
    public int Compare(CardInfo x, CardInfo y)
    {
        var valueComparison = x.Value.CompareTo(y.Value);

        if (valueComparison == 0)
        {
            return x.Kind.CompareTo(y.Kind);
        }

        return valueComparison;
    }
}
#endregion
