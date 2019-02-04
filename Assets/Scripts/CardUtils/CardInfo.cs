using System;
using UnityEngine;

[Serializable]
public struct CardInfo : IEquatable<CardInfo>
{
    public CardKinds Kind;
    public CardValues Value;

    public int Id
    {
        get { return (int)Kind * CardUtils.ValueCount + (int)Value; }
    }

    public string ValueText
    {
        get
        {
            if((int)Value < 1 || (int)Value > 9)
            {
                return Value.ToString();
            }
            else
            {
                return ((int)Value + 1).ToString();
            }
        }
    }

    public Color ValueColor
    {
        get
        {
            if(Kind == CardKinds.Clubs || Kind == CardKinds.Spades)
            {
                return Color.black;
            }
            else
            {
                return Color.red;
            }
        }
    }

    public int CardScore
    {
        get
        {
            return Mathf.Min((int)Value + 1, 10);
        }
    }

    public CardInfo(CardKinds kind, CardValues value)
    {
        Kind = kind;
        Value = value;
    }

    public static CardInfo FromId(int id)
    {
        if(id < 0 || id >= 52)
        {
            throw new Exception("id (" + id + ") is out of range");
        }

        return new CardInfo(CardUtils.GetKindById(id), CardUtils.GetValueById(id));
    }

    public override string ToString()
    {
        return Value + " of " + Kind;
    }

    public bool Equals(CardInfo other)
    {
        return Kind == other.Kind && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
